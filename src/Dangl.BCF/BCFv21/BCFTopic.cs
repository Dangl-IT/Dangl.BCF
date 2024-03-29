using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using Dangl.BCF.BCFv21.Schemas;

namespace Dangl.BCF.BCFv21
{
    /// <summary>
    ///     Single BCFv21 topic container
    /// </summary>
    public class BCFTopic : BindableBase
    {
        private Markup _markup;

        private ObservableCollection<VisualizationInfo> _viewpoints;

        /// <summary>
        ///     Public constructor. Will hook to the Viewpoints CollectionChanged event and automatically
        ///     add viewpoints to the markup if they are not yet present.
        /// </summary>
        public BCFTopic()
        {
            Viewpoints.CollectionChanged += Viewpoints_CollectionChanged;
        }

        /// <summary>
        /// Might holf the raw binary data of a bim snippet attachment
        /// </summary>
        public byte[] SnippetData { get; set; }

        /// <summary>
        ///     The Markup contains the topic's text informations.
        /// </summary>
        public Markup Markup
        {
            get { return _markup; }
            set
            {
                if (SetProperty(ref _markup, value))
                {
                    if (_markup?.Topic != null && string.IsNullOrWhiteSpace(_markup?.Topic?.Guid))
                    {
                        _markup.Topic.Guid = Guid.NewGuid().ToString();
                    }
                    if (_markup?.Topic != null &&  _markup.Topic.CreationDate == default(DateTime))
                    {
                        _markup.Topic.CreationDate = DateTime.UtcNow;
                    }
                }
            }
        }

        /// <summary>
        ///     The List of all the viewpoints within the topic.
        /// </summary>
        public ObservableCollection<VisualizationInfo> Viewpoints
        {
            get
            {
                if (_viewpoints == null)
                {
                    _viewpoints = new ObservableCollection<VisualizationInfo>();
                }
                return _viewpoints;
            }
        }


        private Dictionary<string, byte[]> _viewpointSnapshots { get; set; } = new Dictionary<string, byte[]>();

        /// <summary>
        ///     Links ViewpointGuid and Snapshot
        /// </summary>
        public ReadOnlyDictionary<string, byte[]> ViewpointSnapshots
        {
            get
            {
                return new ReadOnlyDictionary<string, byte[]>(_viewpointSnapshots);
            }
        }

        /// <summary>
        ///     Contains the byte arrays for the actual snapshots. Note: Not implementing property- or collection changed event
        ///     handlers.
        /// </summary>
        public Dictionary<VisualizationInfo, List<byte[]>> ViewpointBitmaps { get; set; } = new Dictionary<VisualizationInfo, List<byte[]>>();

        private void Viewpoints_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (var removedViewpoint in e.OldItems)
                {
                    // Remove snapshots
                    if (ViewpointBitmaps.ContainsKey((VisualizationInfo) removedViewpoint))
                    {
                        ViewpointBitmaps.Remove((VisualizationInfo) removedViewpoint);
                    }
                    // Remove from markup
                    Markup.Viewpoints.Remove(Markup.Viewpoints.First(v => v.Guid == ((VisualizationInfo) removedViewpoint).Guid));
                }
            }
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                if (Markup == null)
                {
                    Markup = new Markup();
                }
                foreach (var addedViewpoint in e.NewItems)
                {
                    // Add only if not already known (For example, when the viewpoint is already present in the markup but has not yet been physically loaded)
                    if (Markup.Viewpoints.Any(v => v.Guid == ((VisualizationInfo) addedViewpoint).Guid))
                    {
                        // Already known, just make sure the viewpoint reference is set correctly.
                        Markup.Viewpoints.First(v => v.Guid == ((VisualizationInfo) addedViewpoint).Guid).Viewpoint = "Viewpoint_" + ((VisualizationInfo) addedViewpoint).Guid + ".bcfv";
                    }
                    else
                    {
                        // Add to markup, viewpoint is not previously known
                        Markup.Viewpoints.Add(new ViewPoint
                        {
                            Guid = ((VisualizationInfo) addedViewpoint).Guid,
                            Viewpoint = "Viewpoint_" + ((VisualizationInfo) addedViewpoint).Guid + ".bcfv"
                        });
                    }
                }
            }
        }

        /// <summary>
        ///     Will de-hook from the Viewpoints CollectionChanged event
        /// </summary>
        protected override void OnDispose()
        {
            Viewpoints.CollectionChanged -= Viewpoints_CollectionChanged;
            base.OnDispose();
        }

        /// <summary>
        /// Adds or updates a snapshots binary data
        /// </summary>
        /// <param name="viewpointGuid"></param>
        /// <param name="snapshotData"></param>
        public void AddOrUpdateSnapshot(string viewpointGuid, byte[] snapshotData)
        {
            if (ViewpointSnapshots.ContainsKey(viewpointGuid))
            {
                _viewpointSnapshots[viewpointGuid] = snapshotData;
            }
            else
            {
                _viewpointSnapshots.Add(viewpointGuid, snapshotData);
                // Add in Markup
                Markup.Viewpoints.First(v => v.Guid == viewpointGuid).Snapshot = "Snapshot_" + viewpointGuid + ".png";
            }
        }

        /// <summary>
        /// Removes a snapshot from this topic
        /// </summary>
        /// <param name="viewpointGuid"></param>
        public void RemoveSnapshot(string viewpointGuid)
        {
            if (ViewpointSnapshots.ContainsKey(viewpointGuid))
            {
                _viewpointSnapshots.Remove(viewpointGuid);
            }
        }
    }
}