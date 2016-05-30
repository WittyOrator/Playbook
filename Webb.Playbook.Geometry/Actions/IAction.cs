using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Webb.Playbook.Geometry.Actions
{
    public interface ITransaction : IDisposable
    {
        IMultiAction AccumulatingAction { get; }
        bool IsDelayed { get; set; }
    }

    public interface IAction
    {
        void Execute();

        void UnExecute();

        /// <summary>
        /// For most Actions, CanExecute is true when ExecuteCount = 0 (not yet executed)
        /// and false when ExecuteCount = 1 (already executed once)
        /// </summary>
        /// <returns>true if an encapsulated action can be applied</returns>
        bool CanExecute();

        /// <summary>
        /// 
        /// </summary>
        /// <returns>true if an action was already executed and can be undone</returns>
        bool CanUnExecute();

        bool TryToMerge(IAction followingAction);
        bool AllowToMergeWithPrevious { get; set; }
    }
}
