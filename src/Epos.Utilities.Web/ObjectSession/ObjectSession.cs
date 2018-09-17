using System;
using System.Collections.Generic;

using Microsoft.AspNetCore.Http;

namespace Epos.Utilities.Web
{
    internal class ObjectSession : IObjectSession
    {
        private const string ObjectSessionKey = "ObjectSession";

        private readonly IHttpContextAccessor myHttpContextAccessor;
        private static readonly Dictionary<Guid, IDictionary<string, object>> mySession =
            new Dictionary<Guid, IDictionary<string, object>>();

        public ObjectSession(IHttpContextAccessor httpContextAccessor) {
            myHttpContextAccessor = httpContextAccessor;
        }

        public object this[string key] {
            get {
                var theGuidString = myHttpContextAccessor.HttpContext.Session.GetString(ObjectSessionKey);
                if (theGuidString == null) {
                    return null;
                }

                Guid theGuid = Guid.Parse(theGuidString);

                var theSession = mySession.Get(theGuid);
                if (theSession == null) {
                    theSession = new Dictionary<string, object>();
                    mySession[theGuid] = theSession;
                }

                return theSession.Get(key);
            }

            set {
                var theGuidString = myHttpContextAccessor.HttpContext.Session.GetString(ObjectSessionKey);
                if (theGuidString == null) {
                    theGuidString = Guid.NewGuid().ToString();
                    myHttpContextAccessor.HttpContext.Session.SetString(ObjectSessionKey, theGuidString);
                }

                Guid theGuid = Guid.Parse(theGuidString);

                var theSession = mySession.Get(theGuid);
                if (theSession == null) {
                    theSession = new Dictionary<string, object>();
                    mySession[theGuid] = theSession;
                }

                theSession[key] = value;
            }
        }
    }
}
