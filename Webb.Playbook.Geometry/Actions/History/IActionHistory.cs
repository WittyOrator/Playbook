using System;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace Webb.Playbook.Geometry.Actions
{
    internal interface IActionHistory : IEnumerable<IAction>, INotifyCollectionChanged 
    {
        bool AppendAction(IAction newAction);
        void Clear();

        void MoveBack();
        void MoveForward();

        bool CanMoveBack { get; }
        bool CanMoveForward { get; }
        int Length { get; }

        SimpleHistoryNode CurrentState { get; }

        IEnumerable<IAction> EnumUndoableActions();
    }
}
