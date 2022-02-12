using System;
using System.Collections.Generic;
using System.Text;

namespace EZmusicL
{
    class MusicResult
    {
        public class MusicItem
        {
            /// <summary>
            /// 
            /// </summary>
            public string name { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string artist { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string url { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string pic { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string lrc { get; set; }
        }


            /// <summary>
            /// 
            /// </summary>
            public List<MusicItem> music { get; set; }

    }
}
