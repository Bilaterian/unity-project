public class Node
{
    public Node leftNode;
    public Node rightNode;
    public Coordinates x;
    public Coordinates z;
    public bool isLeaf = true;
    public Node( Coordinates x, Coordinates z){
        leftNode = null;
        rightNode = null;

        this.x = x;
        this.z = z;
    }
}
