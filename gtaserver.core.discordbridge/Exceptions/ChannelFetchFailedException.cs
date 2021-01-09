using System;
using System.Net;

namespace gtaserver.core.discordbridge.Exceptions
{
    class ChannelFetchFailedException : Exception
    {
        public HttpStatusCode StatusCode { get; set; }

        public ChannelFetchFailedException() 
        { 
        }

        public ChannelFetchFailedException(string message)
            : base(message) 
        { 
        }

        public ChannelFetchFailedException(string message, Exception inner)
            : base(message, inner) 
        {
        }
    }
}
