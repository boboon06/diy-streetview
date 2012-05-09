/*
 * To change this template, choose Tools | Templates
 * and open the template in the editor.
 */
package nodeeditor;

/**
 *
 * @author Chris
 */
public class LatLngNode {
    private LatLng pos;
    private LatLngNode next;
    private String id;

    LatLngNode(LatLng x,String y){
        pos = x;
        id = y;
        next = null;
    }
    
    public LatLng getLatLng(){
        return pos;
    }
    
    public void setNext(LatLngNode x){
        next = x;
    }
    
    public LatLngNode getNext(){
        return next;
    }
    
    public String getID(){
        return id;
    }
     
    public boolean isEquals(LatLngNode x){
     return(pos.isEquals(x.getLatLng()));   
    }
}
