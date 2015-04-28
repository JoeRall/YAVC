﻿using YAVC.Base.Data;
using YAVC.Base.Requests;

namespace YAVC.Base.Commands {
	public class SelectScene : AZoneCommand {

		public Scene Scene { get; protected set; }
		public SelectScene(Zone zone, Scene scene)
			: base(zone) {
			Scene = scene;
		}
		
		protected override SendResult ParseResponseImp(string xml) {
			return SendResult.Empty;
		}

		protected override RequestInfo[] GetRequestInfo() {
			return new RequestInfo[] {
				RequestInfo.GenRequest(yavcMethod.Post, string.Format(@"<YAMAHA_AV cmd=""PUT""><{0}><Scene><Scene_Sel>{1}</Scene_Sel></Scene></{0}></YAMAHA_AV>", TheZone.Name, Scene.Name)),
			};
		}
	}
}
