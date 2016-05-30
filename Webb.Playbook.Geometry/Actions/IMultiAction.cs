using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Webb.Playbook.Geometry.Actions
{
    public interface IMultiAction : IAction, IList<IAction>
    {
        bool IsDelayed { get; set; }
    }

    public class MultiAction : List<IAction>, IMultiAction
    {
        public MultiAction()
        {
            IsDelayed = true;
        }

        public bool IsDelayed { get; set; }

        public void Execute()
        {
            if (!IsDelayed)
            {
                IsDelayed = true;
                return;
            }
            foreach (var action in this)
            {
                action.Execute();
            }
        }

        public void UnExecute()
        {
            foreach (var action in this)
            {
                action.UnExecute();
            }
        }

        public bool CanExecute()
        {
            foreach (var action in this)
            {
                if (!action.CanExecute())
                {
                    return false;
                }
            }
            return true;
        }

        public bool CanUnExecute()
        {
            foreach (var action in this)
            {
                if (!action.CanUnExecute())
                {
                    return false;
                }
            }
            return true;
        }

        public bool TryToMerge(IAction FollowngAction)
        {
            return false;
        }

        private bool mAllowToMergoWithPrevious = false;
        public bool AllowToMergeWithPrevious
        {
            get
            {
                return mAllowToMergoWithPrevious;
            }
            set
            {
                mAllowToMergoWithPrevious = value;
            }
        }
    }
}
