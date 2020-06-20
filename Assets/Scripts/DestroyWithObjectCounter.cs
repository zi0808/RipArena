using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

public class DestroyWithObjectCounter : MonoBehaviour
{
    public int MaxObjects = 100;
    public string Tag = "";
    static int object_counter = 0;
    static Dictionary<string, int> MaxValues = new Dictionary<string, int>();
    static ObservableCollection<DestroyWithObjectCounter> allObjects = null;

    // Start is called before the first frame update
    void Start()
    {
        if (allObjects == null)
        {
            allObjects = new ObservableCollection<DestroyWithObjectCounter>();
            allObjects.CollectionChanged += AllObjects_CollectionChanged;
        }
        if (!MaxValues.ContainsKey(Tag))
        {
            MaxValues.Add(Tag, MaxObjects);
        }

        allObjects.Add(this);
    }

    private void AllObjects_CollectionChanged(object sender,
        System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        if (e.Action != System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            return;
        int TargetMaximum = MaxValues[Tag];

        while (allObjects.Count > TargetMaximum)
        {
            DestroyWithObjectCounter d = allObjects[0];
            d.DoDestroy();
            allObjects.RemoveAt(0);
        }
    }

    public void DoDestroy()
    {
        Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
