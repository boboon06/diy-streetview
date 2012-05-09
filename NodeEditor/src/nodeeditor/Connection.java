/*
 * To change this template, choose Tools | Templates
 * and open the template in the editor.
 */
package nodeeditor;

/**
 *
 * @author Chris
 */
public class Connection {
    private LatLngNode point1;
    private LatLngNode point2;
    private Connection next;
    
    Connection(LatLngNode x,LatLngNode y){
        point1 = x;
        point2 = y;
        next = null;
    }
    
    public LatLngNode getPoint1(){
        return point1;
    }
    
    public LatLngNode getPoint2(){
        return point2;
    }
    
    public void setNext(Connection x){
        next = x;
    }
    
    public Connection getNext(){
        return next;
    }
    
    public boolean isEquals(Connection x){
     LatLngNode point1Temp = x.getPoint1();
     LatLngNode point2Temp = x.getPoint2();
     return((point1.isEquals(point1Temp) && point2.isEquals(point2Temp)) || (point1.isEquals(point2Temp) && point2.isEquals(point1Temp)) );
    }
    
}
