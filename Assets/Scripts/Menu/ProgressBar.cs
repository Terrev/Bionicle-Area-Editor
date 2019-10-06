using UnityEditor;

namespace LOMN.Menu
{
    public class ProgressBar
    {
        public string Title;
        public string Info;
        public float CurrentProgress { get; private set; }
        public readonly float FullProgress;

        public ProgressBar(int fullProgress, string title = "", string info = "")
        {
            Title = title;
            Info = info;
            FullProgress = fullProgress;
        }

        public void Show()
        {
            EditorUtility.DisplayProgressBar(Title, Info, 0);
        }

        public void Hide()
        {
            EditorUtility.ClearProgressBar();
        }

        public void Refresh()
        {
            EditorUtility.DisplayProgressBar(Title+" "+ CurrentProgress + "/"+ FullProgress, Info, CurrentProgress / FullProgress);
        }

        public void Next(bool useRefresh = true)
        {
            ++CurrentProgress;
        }

        public void Reset()
        {
            CurrentProgress = 0;
        }

    }
}