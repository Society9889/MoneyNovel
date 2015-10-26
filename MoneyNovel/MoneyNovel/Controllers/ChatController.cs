using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net;
using PusherServer;
using System.Diagnostics;
using MoneyNovel.Models;
using System.Web.Script.Serialization;

namespace MoneyNovel.Controllers
{
    public class ChatController : BaseController
    {

        //
        // GET: /Chat/MyHistory
        public ActionResult MyHistory()
        {
            ViewBag.title = "My Chat History";
            using (ChatContext db = new ChatContext())
            {
                List<ChatMessage> messages = new List<ChatMessage>();
                List<UserToChatMessage> utcms = db.UserToChatMessages.ToList().Where(t => t.FBID == (string)Session["FBID"]).OrderBy(t => t.ChatMessageID).ToList();
                foreach (UserToChatMessage utcm in utcms)
                {
                    messages.Add(db.ChatMessages.ToList().Where(t => t.ChatMessageID == utcm.ChatMessageID).Last());
                }
                ViewBag.messages = messages;
            }
            return View("ChatHistory");
        }

        //
        // GET: /Chat/MasterHistory
        public ActionResult MasterHistory()
        {
            ViewBag.title = "Master Chat History";
            using (ChatContext db = new ChatContext())
            {
                List<ChatMessage> messages = db.ChatMessages.ToList();
                ViewBag.messages = messages;
            }
            return View("ChatHistory");
        }

        public ActionResult GetRecent()
        {
            List<ChatMessage> messages = new List<ChatMessage>();
            using (ChatContext db = new ChatContext())
            {
                List<UserToChatMessage> utcms = db.UserToChatMessages.ToList().Where(t => t.FBID == (string)Session["FBID"]).OrderBy(t => t.ChatMessageID).ToList();
                foreach (UserToChatMessage utcm in utcms)
                {
                    ChatMessage c = db.ChatMessages.ToList().Where(t => t.ChatMessageID == utcm.ChatMessageID).Last();
                    if(c.TimeSent.CompareTo(Session["LogInTime"]) > 0){
                        messages.Add(c);
                    }
                }
            }
            String json = new JavaScriptSerializer().Serialize(messages);
            return new ContentResult { Content = json, ContentType = "application/json" };
        }

        public ActionResult Auth(string channel_name, string socket_id)
        {
            Debug.WriteLine("trying to auth");
            var pusher = new Pusher("72484", "e9473350e86cf2fd89ac", "3e1cbae89445267f362f");
            var channelData = new PresenceChannelData();
            channelData.user_id = (string) Session["FBID"];
            channelData.user_info = new {
                facebook_id = (string) Session["FBID"]
            };
            var auth = pusher.Authenticate( channel_name, socket_id, channelData );
            var json = auth.ToJson();
            return new ContentResult { Content = json, ContentType = "application/json" };
        }

        [HttpPost]
        public ActionResult SendMessage(String message, String username)
        {
            Debug.WriteLine(username);
            var pusher = new Pusher("72484", "e9473350e86cf2fd89ac", "3e1cbae89445267f362f");
            IGetResult<object> result = pusher.Get<object>("/channels/presence-channel/users");
            PusherUsers users = new JavaScriptSerializer().Deserialize<PusherUsers>(result.Body);
            pusher.Trigger("presence-channel", "my_event", new { message = message, user = Session["FBID"], username = username });

            // Database logging
            using (ChatContext db = new ChatContext())
            {
                // Insert chat message row
                ChatMessage c = new ChatMessage { FBID = (String)Session["FBID"], Message = message, UserName = username, TimeSent = DateTime.Now };
                db.ChatMessages.Add(c);
                db.SaveChanges();   // SaveChanges() is called to get a ChatMessageID value from the DB.
                // Insert user-to-chat rows for all connected users
                foreach(PusherUser user in users.users){
                    UserToChatMessage u = new UserToChatMessage { FBID = user.id.ToString(), ChatMessageID = c.ChatMessageID };
                    db.UserToChatMessages.Add(u);
                }
                db.SaveChanges();
            }

            return new HttpStatusCodeResult((int)HttpStatusCode.OK);
        }
    }
}
