using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using iabi.BCF.BCFv2.Schemas;

/*
 * Note:
 * Due to the missing GUID tag in the VisualizationInfo xsd schema, the VisualizationInfo class has a
 * partial extension defining a GUID to link actual Viewpoints (VisualizationInfo XML instances) with their
 * references in the Markup (Where a list of all viewpoints a topic references is kept)
 *
 */

namespace iabi.BCF.BCFv2
{
    /// <summary>
    ///     Single BCFv2 topic container
    /// </summary>
    public class BCFTopic : BindableBase
    {
        private Markup _Markup;

        private Dictionary<VisualizationInfo, List<byte[]>> _ViewpointBitmaps;

        private ObservableCollection<VisualizationInfo> _Viewpoints;

        /// <summary>
        ///     Public constructor. Will hook to the Viewpoints CollectionChanged event and automatically
        ///     add viewpoints to the markup if they are not yet present.
        /// </summary>
        public BCFTopic()
        {
            Viewpoints.CollectionChanged += Viewpoints_CollectionChanged;
        }

        public byte[] SnippetData { get; set; }

        /// <summary>
        ///     The Markup contains the topic's text informations.
        /// </summary>
        public Markup Markup
        {
            get { return _Markup; }
            set
            {
                if (SetProperty(ref _Markup, value))
                {
                    if (_Markup?.Topic != null && string.IsNullOrWhiteSpace(_Markup?.Topic?.Guid))
                    {
                        _Markup.Topic.Guid = Guid.NewGuid().ToString();
                    }
                    if (Markup?.Topic != null && (_Markup.Topic.CreationDateSpecified || _Markup.Topic.CreationDate == default(DateTime)))
                    {
                        _Markup.Topic.CreationDate = DateTime.UtcNow;
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
                if (_Viewpoints == null)
                {
                    _Viewpoints = new ObservableCollection<VisualizationInfo>();
                }
                return _Viewpoints;
            }
        }

        /// <summary>
        ///     Links ViewpointGuid and Snapshot
        /// </summary>
        private Dictionary<string, byte[]> _ViewpointSnapshots { get; set; }

        public ReadOnlyDictionary<string, byte[]> ViewpointSnapshots
        {
            get
            {
                if (_ViewpointSnapshots == null)
                {
                    _ViewpointSnapshots = new Dictionary<string, byte[]>();
                }
                return new ReadOnlyDictionary<string, byte[]>(_ViewpointSnapshots);
            }
        }

        /// <summary>
        ///     Contains the byte arrays for the actual snapshots. Note: Not implementing property- or collection changed event
        ///     handlers.
        /// </summary>
        public Dictionary<VisualizationInfo, List<byte[]>> ViewpointBitmaps
        {
            get
            {
                if (_ViewpointBitmaps == null)
                {
                    _ViewpointBitmaps = new Dictionary<VisualizationInfo, List<byte[]>>();
                }
                return _ViewpointBitmaps;
            }
            set { _ViewpointBitmaps = value; }
        }

        private void Viewpoints_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (var RemovedViewpoint in e.OldItems)
                {
                    // Remove snapshots
                    if (ViewpointBitmaps.ContainsKey((VisualizationInfo) RemovedViewpoint))
                    {
                        ViewpointBitmaps.Remove((VisualizationInfo) RemovedViewpoint);
                    }
                    // Remove from markup
                    Markup.Viewpoints.Remove(Markup.Viewpoints.First(OldViewpoint => OldViewpoint.Guid == ((VisualizationInfo) RemovedViewpoint).GUID));
                }
            }
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                if (Markup == null)
                {
                    Markup = new Markup();
                }
                foreach (var AddedViewpoint in e.NewItems)
                {
                    // Add only if not already known (For example, when the viewpoint is already present in the markup but has not yet been physically loaded)
                    if (Markup.Viewpoints.Any(KnownViewpoint => KnownViewpoint.Guid == ((VisualizationInfo) AddedViewpoint).GUID))
                    {
                        // Already known, just make sure the viewpoint reference is set correctly.
                        Markup.Viewpoints.First(KnownViewpoint => KnownViewpoint.Guid == ((VisualizationInfo) AddedViewpoint).GUID).Viewpoint = "Viewpoint_" + ((VisualizationInfo) AddedViewpoint).GUID + ".bcfv";
                    }
                    else
                    {
                        // Add to markup, viewpoint is not previously known
                        Markup.Viewpoints.Add(new ViewPoint
                        {
                            Guid = ((VisualizationInfo) AddedViewpoint).GUID,
                            Viewpoint = "Viewpoint_" + ((VisualizationInfo) AddedViewpoint).GUID + ".bcfv"
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

        public void AddOrUpdateSnapshot(string ViewpointGuid, byte[] SnapshotData)
        {
            if (ViewpointSnapshots.ContainsKey(ViewpointGuid))
            {
                _ViewpointSnapshots[ViewpointGuid] = SnapshotData;
            }
            else
            {
                _ViewpointSnapshots.Add(ViewpointGuid, SnapshotData);
                // Add in Markup
                Markup.Viewpoints.First(Curr => Curr.Guid == ViewpointGuid).Snapshot = "Snapshot_" + ViewpointGuid + ".png";
            }
        }

        public void RemoveSnapshot(string ViewpointGuid)
        {
            if (ViewpointSnapshots.ContainsKey(ViewpointGuid))
            {
                _ViewpointSnapshots.Remove(ViewpointGuid);
            }
        }
    }
}