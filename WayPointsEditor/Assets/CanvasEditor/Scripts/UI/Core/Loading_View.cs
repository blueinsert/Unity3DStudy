using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Json;

namespace zPreload
{

	public class Loading_View : MonoBehaviour
	{

		#region Member

		string currentSceneName = "Demo";

		string preloadDataPath {
			get { 
				return "Data/" + currentSceneName + "_preloadData";
			}
		}

		MyObj _myobj;

		MyObj myobj {
			get {
				if (_myobj == null)
					_myobj = GetComponent<MyObj> ();
				return _myobj;
			}
		}

		UIProgressBar _progressBar;

		UIProgressBar progressBar {
			get {
				if (_progressBar == null)
					_progressBar = myobj.GetObj ("ProgressBar").GetComponent<UIProgressBar> ();
				return _progressBar;
			}
		}

		AllPreloadData datas = new AllPreloadData();

		event System.Action preloadFinish_callback;

		#endregion



		#region Interface

		public void LoadToLevel (string _sceneName)
		{
			currentSceneName = _sceneName;
			progressBar.value = 0;

			resetPreloadData ();
			PreloadBegin ();
		}

		public void SetPreloadFinishCallback(System.Action _callback){
			preloadFinish_callback = _callback;
		}

		protected virtual void PreloadBegin ()
		{
			gameObject.SetActive (true);
			StartCoroutine (loadingAssets ());		
		}

		protected virtual void PreloadFinish(){
			gameObject.SetActive (false);
			if (preloadFinish_callback != null)
				preloadFinish_callback ();
		}

		#endregion


		#region Private

		void resetPreloadData(){
			TextAsset text = (TextAsset)Resources.Load<TextAsset> (preloadDataPath);
			if (text != null) {
				JSONNode node = JSONNode.Parse (text.text);
				datas.ResetData (node);
			}
		}

		#endregion

	

		IEnumerator loadingAssets ()
		{
			progressBar.value = datas.Percentage;

			while(!datas.IsFinish){
				datas.LoadNext ();
				yield return null;
			}

			yield return null;
			PreloadFinish ();
		}
	}

	#region 预加载数据

	public class AllPreloadData{
		List<PreloadData> datas = new List<PreloadData>();
		int curIndex = 0; // 当前加载索引
		bool _isFinish;
		public bool IsFinish{
			get{ 
				return _isFinish;
			}
		}
		public float Percentage{
			get{ 
				return (curIndex * 1.0f / datas.Count);
			}
		}

		#region Interface
		public void ResetData(JSONNode _node){
			datas.Clear ();
			JSONNode prefabs = _node ["Prefabs"];
			JSONNode audios = _node ["Audios"];
			JSONNode mbg = _node ["MBG"];
			for(int i = 0;i < prefabs.Count;i++){
				PreloadData _data = new PreloadData (prefabs[i].ToString(), PreloadDataType.enPrefab);
				datas.Add (_data);
			}
			for(int i = 0;i < audios.Count;i++){
				PreloadData _data = new PreloadData (audios[i].ToString(), PreloadDataType.enAudio);
				datas.Add (_data);
			}
			for(int i = 0;i < mbg.Count;i++){
				PreloadData _data = new PreloadData (mbg[i].ToString(), PreloadDataType.enAudio);
				datas.Add (_data);
			}

		}

		public void AddPrefab(string _path){
			PreloadData _data = new PreloadData (_path, PreloadDataType.enPrefab);
			datas.Add (_data);
		}

		public void AddSound(string _path){
			PreloadData _data = new PreloadData (_path, PreloadDataType.enAudio);
			datas.Add (_data);
		}

		public void LoadNext(){
			if (curIndex >= datas.Count) {
				_isFinish = true;
				return;
			}
			Debug.Log ("Loading  : " + datas[curIndex].dataPath );
			Resources.Load (datas[curIndex].dataPath);
			curIndex++;
		}

		public int GetDataCount(){
			return datas.Count;
		}
		public int GetCurrentIndex(){
			return curIndex;
		}


		#endregion




	}

	public class PreloadData{
		public string dataPath;
		PreloadDataType dataType;

		public PreloadData(string _path, PreloadDataType _type){
			dataPath = _path;
			dataType = _type;
		}
	}

	public enum PreloadDataType{
		enPrefab,
		enAudio,
	}

	#endregion

}