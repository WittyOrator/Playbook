using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Webb.Playbook.Geometry.Actions
{
    public abstract class AbstractAction : IAction
    {
        private int mExecuteCount;
        protected int ExecuteCount
        {
            get
            {
                return mExecuteCount;
            }
            set
            {
                mExecuteCount = value;
            }
        }

        public virtual void Execute()
        {
            if (!CanExecute())
            {
                return;
            }
            ExecuteCore();
            ExecuteCount++;
        }

        protected abstract void ExecuteCore();

        public virtual void UnExecute()
        {
            if (!CanUnExecute())
            {
                return;
            }
            UnExecuteCore();
            ExecuteCount--;
        }

        protected abstract void UnExecuteCore();

        public virtual bool CanExecute()
        {
            return ExecuteCount == 0;
        }

        public virtual bool CanUnExecute()
        {
            return !CanExecute();
        }

        /// <summary>
        /// if the last action can be joined with the FollowingAction,
        /// this following action isn't added to the Undo stack,
        /// but rather mixed together with the current one.
        /// </summary>
        /// <param name="FollowingAction"></param>
        /// <returns>true if the FollowingAction can be merged with the 
        /// last last action in the Undo stack</returns>
        public virtual bool TryToMerge(IAction FollowingAction)
        {
            return false;
        }

        private bool mAllowToMergeWithPrevios = true;
        public bool AllowToMergeWithPrevious
        {
            get
            {
                return mAllowToMergeWithPrevios;
            }
            set
            {
                mAllowToMergeWithPrevios = value;
            }
        }
    }
}
