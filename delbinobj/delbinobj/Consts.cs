internal static class Consts
{
    internal struct ExitCodes
    {
        internal const int SUCCESS = 0;

        internal const int ERR_UNKNOWN = 888;
        internal const int ERR_FATAL = 999;

        internal const int ERR_STARTUPDIRREQUIRED = 1001;
        internal const int ERR_INVALIDARGS = 1002;
        internal const int ERR_DIR_NOT_FOUND = 1004;
        internal const int ERR_DELETE_FAILED = 1010;
    }

    internal const string FILENAME_LOG = "delbinobj.log";
}
