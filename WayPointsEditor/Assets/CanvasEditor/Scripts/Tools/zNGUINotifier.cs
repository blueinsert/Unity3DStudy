using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class zNGUINotifyData
{
	public GameObject obj;
	public bool isPress;
	public int index;
}

public class zNGUINotifier
{

	public static void NotifyClick (GameObject _go)
	{
		var baseView = _go.GetComponentInParent<UIBaseView> ();
		if (baseView == null)
			return;

		zNGUINotifyData data = new zNGUINotifyData ();
		data.obj = _go;
		data.isPress = true;
		string sendName = "";
		if (_go.name.StartsWith ("btn_")) {
			var tempString = _go.name.Split ('_');
			sendName = tempString [1];
		} else if (_go.name.StartsWith ("item_")) {
			var tempString = _go.name.Split ('_');
			sendName = tempString [1];
			data.index = int.Parse (tempString [2]);
		}

		if (sendName != "")
			baseView.gameObject.SendMessage (string.Format ("Click_{0}", sendName), data, SendMessageOptions.DontRequireReceiver);
	}

	public static void NotifyPress (GameObject _go, bool _isPress)
	{
		var baseView = _go.GetComponentInParent<UIBaseView> ();
		if (baseView == null)
			return;

		zNGUINotifyData data = new zNGUINotifyData ();
		data.obj = _go;
		data.isPress = _isPress;
		string sendName = "";
		if (_go.name.StartsWith ("btn_")) {
			var tempString = _go.name.Split ('_');
			sendName = tempString [1];
		} else if (_go.name.StartsWith ("item_")) {
			var tempString = _go.name.Split ('_');
			sendName = tempString [1];
			data.index = int.Parse (tempString [2]);
		} else if (_go.name.StartsWith ("sprite_")) {
			var tempString = _go.name.Split ('_');
			sendName = tempString [1];
		} else if (_go.name.StartsWith ("lab_")) {
			var tempString = _go.name.Split ('_');
			sendName = tempString [1];
		}

		if (sendName != "")
			baseView.gameObject.SendMessage (string.Format ("Press_{0}", sendName), data, SendMessageOptions.DontRequireReceiver);
	}

	public static void NotifyLongPress (GameObject _go)
	{
		var baseView = _go.GetComponentInParent<UIBaseView> ();
		if (baseView == null)
			return;

		zNGUINotifyData data = new zNGUINotifyData ();
		data.obj = _go;
		data.isPress = true;
		string sendName = "";
		if (_go.name.StartsWith ("btn_")) {
			var tempString = _go.name.Split ('_');
			sendName = tempString [1];
		} else if (_go.name.StartsWith ("item_")) {
			var tempString = _go.name.Split ('_');
			sendName = tempString [1];
			data.index = int.Parse (tempString [2]);
		} else if (_go.name.StartsWith ("sprite_")) {
			var tempString = _go.name.Split ('_');
			sendName = tempString [1];
		} else if (_go.name.StartsWith ("lab_")) {
			var tempString = _go.name.Split ('_');
			sendName = tempString [1];
		}

		if (sendName != "")
			baseView.gameObject.SendMessage (string.Format ("LongPress_{0}", sendName), data, SendMessageOptions.DontRequireReceiver);
	}

	public static void NotifyDoubleClick(GameObject _go){
		var baseView = _go.GetComponentInParent<UIBaseView> ();
		if (baseView == null)
			return;

		zNGUINotifyData data = new zNGUINotifyData ();
		data.obj = _go;
		data.isPress = true;
		string sendName = "";
		if (_go.name.StartsWith ("btn_")) {
			var tempString = _go.name.Split ('_');
			sendName = tempString [1];
		} else if (_go.name.StartsWith ("item_")) {
			var tempString = _go.name.Split ('_');
			sendName = tempString [1];
			data.index = int.Parse (tempString [2]);
		}

		if (sendName != "")
			baseView.gameObject.SendMessage (string.Format ("DoubleClick_{0}", sendName), data, SendMessageOptions.DontRequireReceiver);
	}

}
