using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BF2D.Combat
{
    public class StatsPreviewController : MonoBehaviour
    {
        [SerializeField] private LayoutGroup container = null;
        [SerializeField] private StatsPreview statsPreviewPrefab = null;

        private List<StatsPreview> statsPreviews = new List<StatsPreview>();
        private int cursorIndex = 0;

        public void Next()
        {
            this.statsPreviews[this.cursorIndex].SetCursor(false);

            if (this.cursorIndex + 1 < this.statsPreviews.Count)
            {
                this.cursorIndex++;
            } 
            else
            {
                this.cursorIndex = 0;
            }

            this.statsPreviews[this.cursorIndex].SetCursor(true);
        }

        public void CreateStatsPreview(string nameKey)
        {
            StatsPreview statsPreview = InstantiateStatsPreview();
            statsPreview.Setup(nameKey);
            this.statsPreviews.Add(statsPreview);
        }

        public void RemoveAllStatsPreviews()
        {
            foreach (StatsPreview s in this.statsPreviews)
            {
                RemoveStatsPreview(s);
            }
        }

        public void RefreshStats()
        {
            foreach (StatsPreview s in this.statsPreviews)
            {
                s.Refresh();
            }
        }

        public void RefreshOrder()
        {
            StatsPreview current = this.statsPreviews[this.cursorIndex];
            this.statsPreviews.Sort((a, b) => GameInfo.Instance.GetPlayer(a.NameKey).Speed.CompareTo(GameInfo.Instance.GetPlayer(b.NameKey).Speed));

            for (int i = 0; i < this.statsPreviews.Count; i++)
            {
                this.statsPreviews[i].transform.SetAsLastSibling();
                if (this.statsPreviews[i] == current)
                {
                    this.cursorIndex = i;
                }
            }
        }

        /*
        private void LoadStatsPreviews()
        {
            this.statsPreviews.Clear();
            StatsPreview[] previews = this.container.GetComponentsInChildren<StatsPreview>();

            foreach (StatsPreview s in previews)
            {
                this.statsPreviews.Add(s);
            }
        }
        */

        private void RemoveStatsPreview(StatsPreview statsPreview)
        {
            this.statsPreviews.Remove(statsPreview);
            Destroy(statsPreview);
        }

        private StatsPreview InstantiateStatsPreview()
        {
            StatsPreview statsPreview = Instantiate(this.statsPreviewPrefab);
            statsPreview.transform.SetParent(this.container.transform);
            statsPreview.transform.localScale = Vector3.one;
            return statsPreview;
        }
    }
}
