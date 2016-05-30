using System;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace Webb.Playbook.Geometry.Actions
{
    /// <summary>
    /// IActionHistory represents a recorded list of actions undertaken by user
    /// 
    /// This class implements a usual, linear action sequence. You can move back and forth
    /// changing the state of the respective document. When you move forward, you execute
    /// a respective action, when you move backward, you Undo it (UnExecute).
    /// 
    /// Implemented through a double linked-list of SimpleHistoryNode objects.
    /// </summary>
    internal class SimpleHistory : IActionHistory
    {
        public SimpleHistory()
        {
            Init();
        }

        #region Events
        public event NotifyCollectionChangedEventHandler CollectionChanged;
        protected void RaiseUndoBufferChanged()
        {
            if (CollectionChanged != null)
            {
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }
        }
        #endregion

        private SimpleHistoryNode mCurrentState = new SimpleHistoryNode();
        public SimpleHistoryNode CurrentState
        {
            get
            {
                return mCurrentState;
            }
            set
            {
                if (value != null)
                {
                    mCurrentState = value;
                }
                else
                {
                    throw new ArgumentNullException("CurrentState");
                }
            }
        }

        private SimpleHistoryNode mHead;
        public SimpleHistoryNode Head
        {
            get
            {
                return mHead;
            }
            set
            {
                mHead = value;
            }
        }

        private IAction mLastAction;
        public IAction LastAction
        {
            get
            {
                return mLastAction;
            }
            set
            {
                mLastAction = value;
            }
        }

        /// <summary>
        /// Adds a new action to the tail after current state. If
        /// there exist more actions after this, they're lost (Garbage Collected).
        /// This is the only method of this class that actually modifies the linked-list.
        /// </summary>
        /// <param name="newAction">Action to be added.</param>
        /// <returns>true if action was appended, false if it was merged with the previous one</returns>
        public bool AppendAction(IAction newAction)
        {
            if (CurrentState.PreviousAction == null)
            {// don't need merge
                CurrentState.NextAction = newAction;
                CurrentState.NextNode = new SimpleHistoryNode(newAction, CurrentState);
            }
            else
            {// need merge
                if (CurrentState.PreviousAction.TryToMerge(newAction))
                {//merge
                    RaiseUndoBufferChanged();
                    return false;
                }
                else
                {
                    CurrentState.NextAction = newAction;
                    CurrentState.NextNode = new SimpleHistoryNode(newAction, CurrentState);
                }
            }
            return true;
        }

        /// <summary>
        /// All existing Nodes and Actions are garbage collected.
        /// </summary>
        public void Clear()
        {
            Init();
            RaiseUndoBufferChanged();
        }

        private void Init()
        {
            CurrentState = new SimpleHistoryNode();
            Head = CurrentState;
        }

        public IEnumerable<IAction> EnumUndoableActions()
        {
            SimpleHistoryNode Current = Head;
            while (Current != null && Current != CurrentState && Current.NextAction != null)
            {
                yield return Current.NextAction;
                Current = Current.NextNode;
            }
        }

        public bool CanMoveForward
        {
            get
            {
                return CurrentState.NextAction != null &&
                    CurrentState.NextNode != null;
            }
        }

        public bool CanMoveBack
        {
            get
            {
                return CurrentState.PreviousAction != null &&
                    CurrentState.PreviousNode != null;
            }
        }

        private int mLength = 0;
        /// <summary>
        /// The length of Undo buffer (total number of undoable actions)
        /// </summary>
        public int Length
        {
            get
            {
                return mLength;
            }
            set
            {
                mLength = value;
            }
        }

        public IEnumerator<IAction> GetEnumerator()
        {
            return EnumUndoableActions().GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void MoveForward()
        {
            if (CanMoveForward)
            {
                CurrentState.NextAction.Execute();
                CurrentState = CurrentState.NextNode;
                Length += 1;
                RaiseUndoBufferChanged();
            }
        }

        public void MoveBack()
        {
            if (CanMoveBack)
            {
                CurrentState.PreviousAction.UnExecute();
                CurrentState = CurrentState.PreviousNode;
                Length -= 1;
                RaiseUndoBufferChanged();
            }
        }
    }
}
