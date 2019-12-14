using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;


public class TestApp {
  public static void Main() {
    var list = new FastList<int>();
    list.Add(1);
    list.Add(2);
    list.Add(3);
    list.Add(4);
    list.Add(5);
    Console.WriteLine(list.Count);
    Console.WriteLine(list.Capacity);
  }
}

//List that can remove elements in O(1).
public class FastList<T>
{
    private const int CAP = 16;

    protected bool[] SlotUsed;
    protected T[] Storage;
    protected int Begin;
    protected int End;
    public int Capacity { get; private set; }
    public int Count { get; private set; }


    // Random access
    public T this[int index]
    {
        get
        {
            int current = Begin;
            int count = 0;
            while (count < index)
            {
                if (SlotUsed[current])
                    count++;
                current++;
            }
            return Storage[current];
        }
        set
        {
            int current = Begin;
            int count = 0;
            while (count < index)
              {
                  if (SlotUsed[current])
                      count++;
                  current++;
              }
              Storage[current] = value;
          }
      }

      public FastList(int capacity = -1)
      {
          Clear(capacity < 2 ? CAP : capacity);
      }

      public FastList(IEnumerable<T> enumerable)
      {
          List<T> list = enumerable.ToList();
          int capacity = CAP;
          while((capacity *= 2) < list.Count) { }

          Clear(capacity);

          for (int i = 0; i < list.Count; i++)
          {
              SlotUsed[i] = true;
              Storage[i] = list[i];
          }
      }

      public virtual void Add(T item)
      {
          SlotUsed[End] = true;
          Storage[End++] = item;
          Count++;
          if (End != Capacity)
          {
              return;
          }
          Capacity *= 2;
          bool[] newSlotUsed = new bool[Capacity];
          T[] newStorage = new T[Capacity];
          int count = 0;
          for (int i = 0; i < SlotUsed.Length; i++)
          {
              if(SlotUsed[i])
              {
                  newSlotUsed[count] = true;
                  newStorage[count] = Storage[i];
                  count++;
              }
          }

          Begin = 0;
          End = count;
          Storage = newStorage;
          SlotUsed = newSlotUsed;
      }

      public virtual void Clear()
      {
          Capacity = CAP;
          Clear(Capacity);
      }

      private void Clear(int capacity)
      {
          Capacity = capacity;
          Begin = 0;
          End = 0;
          Count = 0;
          SlotUsed = new bool[Capacity];
          Storage = new T[Capacity];
      }

      public bool Contains(T item)
      {
          for (int i = Begin; i < End; i++)
          {
              if (SlotUsed[i] && Equals(item, Storage[i]))
                  return true;
          }
          return false;
      }

      public void CopyTo(T[] array, int arrayIndex)
      {
          foreach (T item in this)
              array[arrayIndex++] = item;
      }

      public virtual bool Remove(T item)
      {
          for (int i = Begin; i < End; i++)
          {
              if(SlotUsed[i] && Equals(item, Storage[i]))
              {
                  SlotUsed[i] = false;
                  Count--;
                  return true;
              }
          }
          return false;
      }

      public void AddRange(IEnumerable<T> enumerable)
      {
          foreach (T item in enumerable)
              Add(item);
      }

      public IEnumerator<T> GetEnumerator()
      {
          return new FastListEnumerator<FastList<T>>(this);
      }

  //Enumerator
      protected internal class FastListEnumerator<TListType> : IEnumerator<T>
          where TListType : FastList<T>
      {

          protected readonly TListType List;
          protected int Counter;
          public FastListEnumerator(TListType list)
          {
              List = list;
              Counter = List.Begin - 1;
          }

          public void Dispose()
          {
              Counter = int.MaxValue - 10;
          }

          public virtual bool MoveNext()
          {
              while(++Counter < List.End && !List.SlotUsed[Counter])
              { }
              return Counter < List.End;
          }

          public virtual void Reset()
          {
              Counter = List.Begin - 1;
          }

          public T Current => List.Storage[Counter];

          object IEnumerator.Current => Current;
      }
  }

