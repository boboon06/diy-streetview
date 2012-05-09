/*
 * To change this template, choose Tools | Templates
 * and open the template in the editor.
 */
package nodeeditor;

/**
 *
 * @author Chris
 */
public class NodeList {
    private LatLngNode head =  null;
    
    public void addNode(LatLng x, String y){
        LatLngNode temp = new LatLngNode(x,y);
        temp.setNext(head);
        head = temp;
    }
    
    
}
