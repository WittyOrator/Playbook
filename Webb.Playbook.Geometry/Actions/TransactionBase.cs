using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Webb.Playbook.Geometry.Actions
{
    public class TransactionBase : ITransaction
    {
        #region ctors

        public TransactionBase(ActionManager am, bool isDelayed)
            : this(am)
        {
            IsDelayed = isDelayed;
        }

        public TransactionBase(ActionManager am)
            : this()
        {
            ActionManager = am;
            if (am != null)
            {
                am.OpenTransaction(this);
            }
        }

        public TransactionBase()
        {
            IsDelayed = true;
        }

        public bool IsDelayed { get; set; }

        #endregion

        #region MultiAction
        protected IMultiAction mAccumulatingAction;
        public IMultiAction AccumulatingAction
        {
            get
            {
                return mAccumulatingAction;
            }
        }
        #endregion

        #region Commit, Rollback

        public virtual void Commit()
        {
            if (ActionManager != null)
            {
                ActionManager.CommitTransaction();
            }
        }

        public virtual void Rollback()
        {
            if (ActionManager != null)
            {
                ActionManager.RollBackTransaction();
            }
        }

        #endregion

        #region ActionManager

        private ActionManager mActionManager;
        public ActionManager ActionManager
        {
            get
            {
                return mActionManager;
            }
            private set
            {
                if (value == null)
                {
                }

                mActionManager = value;
            }
        }

        #endregion

        #region Dispose

        public void Dispose()
        {
            Commit();
        }

        #endregion
    }
}
