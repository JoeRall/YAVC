﻿using YAVC.Base.Data;
using YAVC.Base.Requests;

namespace YAVC.Base.Commands {
	public class ToggleMute : AZoneCommand {

		public ToggleMute(Zone zone)
			: base(zone) {
				zone.Volume.ToggleMute();
		}

		protected override SendResult ParseResponseImp(string xml) {
			if (xml.Contains("Mute")) {
				return SendResult.Succcess;
			}
			return SendResult.Empty;
		}

		protected override RequestInfo[] GetRequestInfo() {
			return new RequestInfo[] {
		      RequestInfo.GenRequest(yavcMethod.Post, RequestString),
		    };
		}

		private string RequestString {
			get {
				return string.Format(@"<YAMAHA_AV cmd=""PUT""><{0}><Volume><Mute>{1}</Mute></Volume></{0}></YAMAHA_AV>",
						TheZone.Name,
						TheZone.Volume.MuteOn ? "On" : "Off");
			}
		}
	}
}
