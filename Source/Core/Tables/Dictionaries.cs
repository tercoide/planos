 public class DictList
    {
        /// <summary>Entry name (DXF group code 3)</summary>
        public string Name { get; set; }

        /// <summary>Soft-owner ID/handle to entry object (DXF group code 350)</summary>
        public string IdSoftOwner { get; set; }

        public DictList() { }

        public DictList(string name, string idSoftOwner = null)
        {
            Name = name;
            IdSoftOwner = idSoftOwner;
        }

        public override string ToString() => $"DictList(Name={Name}, IdSoftOwner={IdSoftOwner})";
    }

    /// <summary>
    /// Converted from Gambas DictEntry.class
    /// Original DXF/group-code notes kept in comments for reference.
    /// </summary>
    public class DictEntry
    {
        /// <summary>
        /// Entry name (DXF group code 3)
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Handle / identifier (DXF group code 5)
        /// </summary>
        public string Id { get; set; }

        // public string IdOwner { get; set; } // commented out in original

        /// <summary>
        /// Hard-owner flag (DXF group code 280). If set to 1, elements are hard-owned.
        /// </summary>
        public int HardOwnerFlag { get; set; }

        /// <summary>
        /// Duplicate record cloning flag (DXF group code 281).
        /// 0 = Not applicable
        /// 1 = Keep existing
        /// 2 = Use clone
        /// 3 = &lt;xref&gt;$0$&lt;name&gt;
        /// 4 = $0&lt;name&gt;
        /// 5 = Unmangle name
        /// </summary>
        public int DuplicateRecord { get; set; }

        /// <summary>
        /// Soft-owner ID/handle to entry object (DXF group code 350)
        /// </summary>
        public string IdSoftOwner { get; set; }

        /// <summary>
        /// Items associated with this dict entry (original Gambas: New DictList[]).
        /// Use a List for mutability in C#.
        /// </summary>
        public List<DictList> Items { get; } = new List<DictList>();

        public DictEntry() { }

        public DictEntry(string name, string id = null)
        {
            Name = name;
            Id = id;
        }

        /// <summary>
        /// Adds a DictList item to this entry.
        /// </summary>
        public void AddItem(DictList item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            Items.Add(item);
        }

        /// <summary>
        /// Removes the first item with the given name. Returns true when removed.
        /// </summary>
        public bool RemoveItemByName(string itemName)
        {
            var it = Items.FirstOrDefault(i => i.Name == itemName);
            if (it == null) return false;
            return Items.Remove(it);
        }

        /// <summary>
        /// Finds an item by name (case-sensitive). Returns null if not found.
        /// </summary>
        public DictList FindItemByName(string itemName) =>
            Items.FirstOrDefault(i => i.Name == itemName);

        public override string ToString()
        {
            return $"DictEntry(Name={Name}, Id={Id}, HardOwnerFlag={HardOwnerFlag}, DuplicateRecord={DuplicateRecord}, Items={Items.Count})";
        }
    }