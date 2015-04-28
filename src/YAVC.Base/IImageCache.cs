using System;
using System.IO;

namespace YAVC.Base {
	public interface IImageCache {
		void GetImage(string imageUri, Action<Stream> OnGetImage);
		void DownloadImage(string imageUri, Action onFinished);
	}
}
