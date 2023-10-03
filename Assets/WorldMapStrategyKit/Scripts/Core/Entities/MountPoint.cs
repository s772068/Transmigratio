﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace WorldMapStrategyKit {

    /// <summary>
    /// Mount Point record. Mount points are stored in the mountPoints file, in packed string editable format inside Resources/Geodata folder.
    /// </summary>
    public partial class MountPoint : IExtendableAttribute {

        /// <summary>
        /// Name of this mount point.
        /// </summary>
        public string name;

        /// <summary>
        /// Type of mount point. This is an optional, user-defined integer value.
        /// </summary>
        public int type;

        /// <summary>
        /// The index of the country.
        /// </summary>
        public int countryIndex;

        /// <summary>
        /// The index of the province or -1 if the mount point is not linked to any province.
        /// </summary>
        public int provinceIndex;

        /// <summary>
        /// The location of the mount point on the sphere.
        /// </summary>
        public Vector2 unity2DLocation;

        /// <summary>
        /// An unique identifier useful to persist data between sessions. Used by serialization.
        /// </summary>
        /// <value>The unique identifier.</value>
        public int uniqueId { get; set; }

        /// <summary>
        /// Use this property to add/retrieve custom attributes for this country
        /// </summary>
        public JSONObject attrib { get; set; }

        public MountPoint(string name, int countryIndex, int provinceIndex, Vector2 unity2DLocation, int uniqueId, int type) {
            this.name = name;
            this.countryIndex = countryIndex;
            this.provinceIndex = provinceIndex;
            this.unity2DLocation = unity2DLocation;
            this.type = type;
            this.uniqueId = uniqueId;
            this.attrib = new JSONObject();
        }

        public MountPoint Clone() {
            MountPoint c = new MountPoint(name, countryIndex, provinceIndex, unity2DLocation, uniqueId, type);
            c.attrib = new JSONObject();
            c.attrib.Add(attrib);
            return c;
        }
    }


    [Serializable]
    public struct MountPointJSON {
        public string name;
        public int type;
        public string country;
        public string province;
        public int uniqueId;
        public JSONObject attrib;
        public Vector2 unity2DLocation;
    }

    [Serializable]
    public class MountPointsJSONData {
        public List<MountPointJSON> mountPoints = new List<MountPointJSON>();
    }
}
