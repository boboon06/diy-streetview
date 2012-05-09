/*
 * To change this template, choose Tools | Templates
 * and open the template in the editor.
 */
package nodeeditor;

/**
 *
 * @author Chris
 */
public class LatLng {
    private float lat;
    private float lng;
    
    LatLng(float x,float y){
        lat = x;
        lng = y;
    }
    
    public float getLat(){
        return lat;
    }
            
    public float getLng(){
        return lng;
    }
    
    public boolean isEquals(LatLng x){
        float tempLat = x.getLat();
        float tempLng = x.getLng();
        
        return(lat == tempLat && lng == tempLng);
    }
}
