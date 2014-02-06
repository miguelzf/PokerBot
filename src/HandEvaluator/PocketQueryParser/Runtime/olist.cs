using System.Collections;

/// <exclude/>
public class ObjectList
{
	class Link {
		internal object it;
		internal Link next;
		internal Link(object o, Link x) { it=o;next=x; }
	}		
	void Add0(Link a) {
		if (head==null)
			head = last = a;
		else
			last = last.next = a;
	}
	object Get0(Link a,int x) { 
		if (a==null || x<0)  // safety
			return null;
		if (x==0)
			return a.it;
		return Get0(a.next,x-1);
	}
	private Link head = null, last=null;
	private int count = 0;
    /// <exclude/>
	public ObjectList() {}
    /// <exclude/>
	public void Add(object o) { Add0(new Link(o,null)); count++; }
    /// <exclude/>
	public void Push(object o) { head = new Link(o,head); count++; }
    /// <exclude/>
	public object Pop() { object r=head.it; head=head.next; count--; return r; }
    /// <exclude/>
    public object Top { get { return head.it; }}
    /// <exclude/>
	public int Count { get { return count; }}
    /// <exclude/>
	public object this[int ix] { get { return Get0(head,ix); } }
    /// <exclude/>
	public class OListEnumerator : IEnumerator
	{
		ObjectList list;
		Link cur = null;
        /// <exclude/>
		public object Current { get { return cur.it; }}
        /// <exclude/>
		public OListEnumerator(ObjectList o)
		{
			list = o;
		}
        /// <exclude/>
		public bool MoveNext()
		{
			if (cur==null)
				cur = list.head;
			else
				cur = cur.next;
			return cur!=null;
		}
        /// <exclude/>
		public void Reset()
		{
			cur = null;
		}
	}
    /// <exclude/>
	public IEnumerator GetEnumerator()
	{
		return new OListEnumerator(this);
	}
}
