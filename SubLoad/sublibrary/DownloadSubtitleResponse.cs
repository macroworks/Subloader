﻿namespace SubLib
{
    public class Subcontents
    {
        public string idsubtitlefile;
        public string data;
    }

    public class DownloadSubtitleResponse
    {
        public Subcontents[] data;
        public double seconds;
    }
}