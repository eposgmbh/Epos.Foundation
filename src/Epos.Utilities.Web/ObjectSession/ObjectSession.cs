using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

using Microsoft.AspNetCore.Http;

namespace Epos.Utilities.Web
{
    internal class ObjectSession : IObjectSession
    {
        private const string ObjectSessionKey = "ObjectSession";

        private readonly IHttpContextAccessor myHttpContextAccessor;
        private static readonly IDictionary<Guid, IDictionary<string, object>> mySession =
            new ConcurrentDictionary<Guid, IDictionary<string, object>>();

        public ObjectSession(IHttpContextAccessor httpContextAccessor) {
            myHttpContextAccessor = httpContextAccessor;
        }

        public object this[string key] {
            get {
                string theGuidString = myHttpContextAccessor.HttpContext.Session.GetString(ObjectSessionKey);
                if (theGuidString == null) {
                    return null;
                }

                var theGuid = Guid.Parse(theGuidString);

                IDictionary<string, object> theSession = mySession.Get(theGuid);
                if (theSession == null) {
                    theSession = new Dictionary<string, object>();
                    mySession[theGuid] = theSession;
                }

                return theSession.Get(key);
            }

            set {
                string theGuidString = myHttpContextAccessor.HttpContext.Session.GetString(ObjectSessionKey);
                if (theGuidString == null) {
                    theGuidString = Guid.NewGuid().ToString();
                    myHttpContextAccessor.HttpContext.Session.SetString(ObjectSessionKey, theGuidString);
                }

                var theGuid = Guid.Parse(theGuidString);

                IDictionary<string, object> theSession = mySession.Get(theGuid);
                if (theSession == null) {
                    theSession = new Dictionary<string, object>();
                    mySession[theGuid] = theSession;
                }

                theSession[key] = value;
            }
        }
    }
}
