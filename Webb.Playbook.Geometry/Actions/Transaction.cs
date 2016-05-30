using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Webb.Playbook.Geometry.Actions
{
    public class Transaction : TransactionBase
    {
        private Transaction(ActionManager am, bool delayed)
            : base(am, delayed)
        {
            this.mAccumulatingAction = new MultiAction();
        }

        public static Transaction Create(ActionManager am, bool delayed)
        {
            return new Transaction(am, delayed);
        }

        public override void Commit()
        {
            this.AccumulatingAction.IsDelayed = this.IsDelayed;
            base.Commit();
        }
    }
}
