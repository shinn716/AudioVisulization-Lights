@script ExecuteInEditMode()
// Scroll main texture based on time

var scrollSpeed = 1.0;
var MainoffsetX = 0.0;
var MainoffsetY = 0.0;

var UseCustomTex = false;
var CustomTexName = "";

function Update () 
{
    var offset = Time.time * scrollSpeed;
    if(UseCustomTex){
	GetComponent.<Renderer>().sharedMaterial.SetTextureOffset (CustomTexName, Vector2(MainoffsetX*offset, MainoffsetY*offset));
    }
    else{
    GetComponent.<Renderer>().sharedMaterial.SetTextureOffset ("_MainTex", Vector2(MainoffsetX*offset, MainoffsetY*offset));
    
    }
}