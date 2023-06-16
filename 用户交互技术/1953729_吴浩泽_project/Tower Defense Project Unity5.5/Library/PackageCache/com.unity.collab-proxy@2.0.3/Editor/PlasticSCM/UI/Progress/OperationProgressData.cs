namespace Unity.PlasticSCM.Editor.UI.Progress
{
    internal class OperationProgressData
    {
        internal string ProgressHeader
        {
            get
            {
                lock (mLockGuard)
                {
                    return mProgressHeader;
                }
            }
            set
            {
                lock (mLockGuard)
                {
                    mProgressHeader = value;
                }
            }
        }

        internal string TotalProgressMessage
        {
            get
            {
                lock (mLockGuard)
                {
                    return mTotalProgressMessage;
                }
            }
            set
            {
                lock (mLockGuard)
                {
                    mTotalProgressMessage = value;
                }
            }
        }

        internal string CurrentBlockProgressMessage
        {
            get
            {
                lock (mLockGuard)
                {
                    return mBlockProgressMessage;
                }
            }
            set
            {
                lock (mLockGuard)
                {
                    mBlockProgressMessage = value;
                }
            }
        }

        internal double TotalProgressPercent
        {
            get
            {
                lock (mLockGuard)
                {
                    return mTotalProgressPercent;
                }
            }
            set
            {
                lock (mLockGuard)
                {
                    mTotalProgressPercent = value;
                }
            }
        }

        internal double CurrentBlockProgressPercent
        {
            get
            {
                lock (mLockGuard)
                {
                    return mBlockProgressPercent;
                }
            }
            set
            {
                lock (mLockGuard)
                {
                    mBlockProgressPercent = value;
                }
            }
        }

        internal bool ShowCurrentBlock
        {
            get
            {
                lock (mLockGuard)
                {
                    return mShowCurrentBlock;
                }
            }
            set
            {
                lock (mLockGuard)
                {
                    mShowCurrentBlock = value;
                }
            }
        }

        internal bool CanCancelProgress
        {
            get
            {
                lock (mLockGuard)
                {
                    return mCanCancelProgress;
                }
            }
            set
            {
                lock (mLockGuard)
                {
                    mCanCancelProgress = value;
                }
            }
        }

        internal void ResetProgress()
        {
            lock (mLockGuard)
            {
                mProgressHeader = string.Empty;
                mTotalProgressMessage = string.Empty;
                mBlockProgressMessage = string.Empty;
                mTotalProgressPercent = 0;
                mBlockProgressPercent = 0;
                mShowCurrentBlock = false;
                mCanCancelProgress = false;
            }
        }

        string mProgressHeader;
        string mTotalProgressMessage;
        string mBlockProgressMessage;
        double mTotalProgressPercent;
        double mBlockProgressPercent;
        bool mShowCurrentBlock;
        bool mCanCancelProgress;

        object mLockGuard = new object();
    }
}
