using System.Collections.Generic;
using System;

[Serializable] public class ObjectState { public string id; public bool destroyed; public bool opened; }
[Serializable] public class SaveData { public Dictionary<string, ObjectState> objects = new(); }
