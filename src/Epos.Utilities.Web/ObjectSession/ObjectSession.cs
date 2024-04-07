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
        private static readonly IDictionary<Guid, IDictionary<string, object?>> Session =
            new ConcurrentDictionary<Guid, IDictionary<string, object?>>();

        public ObjectSession(IHttpContextAccessor httpContextAccessor) {
            myHttpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        public object? this[string key] {
            get {
                string theGuidString = myHttpContextAccessor.HttpContext.Session.GetString(ObjectSessionKey);
                if (theGuidString is null) {
                    return null;
                }

                var theGuid = Guid.Parse(theGuidString);

#pragma warning disable CS8620 // Argument cannot be used for parameter due to differences in the nullability of reference types.
                IDictionary<string, object?>? theSession = Session.Get(theGuid);
#pragma warning restore CS8620 // Argument cannot be used for parameter due to differences in the nullability of reference types.
                if (theSession is null) {
                    theSession = new Dictionary<string, object?>();
                    Session[theGuid] = theSession;
                }

                return theSession.Get(key);
            }

            set {
                string theGuidString = myHttpContextAccessor.HttpContext.Session.GetString(ObjectSessionKey);
                if (theGuidString is null) {
                    theGuidString = Guid.NewGuid().ToString();
                    myHttpContextAccessor.HttpContext.Session.SetString(ObjectSessionKey, theGuidString);
                }

                var theGuid = Guid.Parse(theGuidString);

#pragma warning disable CS8620 // Argument cannot be used for parameter due to differences in the nullability of reference types.
                IDictionary<string, object?>? theSession = Session.Get(theGuid);
#pragma warning restore CS8620 // Argument cannot be used for parameter due to differences in the nullability of reference types.
                if (theSession is null) {
                    theSession = new Dictionary<string, object?>();
                    Session[theGuid] = theSession;
                }

                theSession[key] = value;
            }
        }
    }
}
