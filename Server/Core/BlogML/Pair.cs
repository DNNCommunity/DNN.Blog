
namespace DotNetNuke.Modules.Blog.BlogML
{

  /// <summary>
 /// A serializable keyvalue pair class
 /// </summary>
  public struct Pair<K, V>
  {
    public K Key;
    public V Value;
    public Pair(K key, V value)
    {
      Key = key;
      Value = value;
    }
  }

}