/*
 * To change this template, choose Tools | Templates
 * and open the template in the editor.
 */
package nodeeditor;

/**
 *
 * @author Chris
 */
public class ConnectionList {
    
    private Connection head;
    
    ConnectionList(){
        head = null;
    }
    
    public void addConnection(LatLngNode point1,LatLngNode point2){
        Connection temp = new Connection(point1,point2);
        temp.setNext(head);
        head = temp;
    }
    
    public void removeConnection(int x){
        Connection temp = head;
        if(x == 1){
            head = temp.getNext();
            temp.setNext(null);
        }
        else
        for(int i = 0; i<x-1;i++){
            temp.getNext();
        }
        Connection temp2 = temp.getNext();
        temp.setNext(temp2.getNext());
        temp2.setNext(null);
    }
    
    public boolean hasConnection(Connection temp){
        if (head == null)
            return false;
        Connection headTemp = head.getNext();
        if (headTemp.isEquals(temp))
            return true;
        while((headTemp != null) || !headTemp.isEquals(temp))
            headTemp.getNext();
        return (headTemp.isEquals(temp));
    }
}
