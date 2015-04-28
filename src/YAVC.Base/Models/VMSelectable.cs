using System;
using YAVC.Base.Data;

namespace YAVC.Base.Models {
	/// <summary>
	/// This class is designed to be able to provide enough
	/// info to select a Zone, Input, Scene or DSP program.
	/// </summary>
	public class VMSelectable : AVM, IComparable {

        public string ImageUri
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }


        public bool IsSelected
        {
            get { return GetValue<bool>(); }
            set { SetValue(value); }
        }

        public string DisplayName
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        public VMSelectableType SelectionType
        {
            get { return GetValue<VMSelectableType>(); }
            set { SetValue(value); }
        }


        public string SelectString
        {
            get { return GetValue<string>(); }
            private set { SetValue(value); }
        }

        public VMMain MainVM
        {
            get { return GetValue<VMMain>(); }
            private set { SetValue(value); }
        }
		
        
        protected VMSelectable(IController c, VMMain mainVM)
			: base(c) {
			MainVM = mainVM;
		}
		
        public VMSelectable(IController c, VMMain mainVM, Zone zone, string imageUri)
			: this(c, mainVM) {
			SelectString = GetSelectString(zone);
			DisplayName = zone.DisplayName;
			ImageUri = imageUri;
			SelectionType = VMSelectableType.Zone;
		}

		public VMSelectable(IController c, VMMain mainVM, Scene s)
			: this(c, mainVM) {
			SelectString = GetSelectString(s);
			DisplayName = s.DisplayName;
			ImageUri = GetImageUri(s, c);
			SelectionType = VMSelectableType.Scene;
		}
		public VMSelectable(IController c, VMMain mainVM, Input i)
			: this(c, mainVM) {
			SelectString = GetSelectString(i);
			DisplayName = i.Title;
			ImageUri = GetImageUri(i, c);
			SelectionType = VMSelectableType.Input;
		}
		public VMSelectable(IController c, VMMain mainVM, string name, VMSelectableType type)
			: base(c) {
			DisplayName = SelectString = name;
			MainVM = mainVM;
			SelectionType = type;
		}

		public void Select() {
			switch (SelectionType) {
				case VMSelectableType.Zone:
					MainVM.SelectZone(SelectString);
					break;
				case VMSelectableType.Scene:
					MainVM.SelectScene(SelectString);
					break;
				case VMSelectableType.DSP:
					MainVM.SelectDSP(SelectString);
					break;
				case VMSelectableType.Input:
					MainVM.SelectInput(SelectString);
					break;
				default:
					break;
			}

			//-- Scenes can be selected, but their selection state
			// is not preserved liked the other selectable's.
			// This recreates the experience found on the iOS platform.
			if(SelectionType != VMSelectableType.Scene)
				IsSelected = true;
		}

		private static string GetImageUri(Scene s, IController controller) {
			return GetImageUri(s.IconRelativeUri, controller);
		}
		private static string GetImageUri(Input z, IController controller) {
			return GetImageUri(z.IconOn, controller);
		}
		private static string GetImageUri(string relativeUri, IController controller) {
			return string.Format("http://{0}{1}", controller.HostNameorAddress, relativeUri);
		}

		public static string GetSelectString(Zone zone) { return zone.Name; }
		public static string GetSelectString(Input i) { return i.Param; }
		public static string GetSelectString(Scene s) { return s.Name; }

		#region IComparable Members

		public int CompareTo(object obj) {
			var sel = obj as VMSelectable;
			if (null == sel) return -1;

			return SelectString.CompareTo(sel.SelectString);
		}

		#endregion
	}

	public enum VMSelectableType {
		Zone,
		Scene,
		DSP,
		Input
	}
}
