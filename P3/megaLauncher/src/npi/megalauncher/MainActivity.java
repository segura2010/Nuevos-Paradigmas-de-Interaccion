package npi.megalauncher;

import android.app.Activity;
import android.content.Intent;
import android.content.pm.ApplicationInfo;
import android.gesture.Gesture;
import android.gesture.GestureOverlayView;
import android.gesture.Prediction;
import android.net.Uri;
import android.os.Bundle;
import android.gesture.GestureLibraries;
import android.gesture.GestureLibrary;
import android.util.Log;
import android.view.Menu;
import android.widget.Toast;
import android.os.Environment;
import java.util.ArrayList;
import java.io.File;
import java.util.List;

public class MainActivity extends Activity {
  public static GestureLibrary gesturelib = null;
    //private TextView gesturesText = null;
  
  @Override
  protected void onCreate(Bundle savedInstanceState) 
  {
	  super.onCreate(savedInstanceState);
	  setContentView(R.layout.activity_main);
	  // Define la biblioteca de gestos a usar
	  gesturelib = GestureLibraries.fromRawResource(this, R.raw.gestures);
	  //String path = new File(Environment.getExternalStorageDirectory(),"gestures").getAbsolutePath();
	  //gesturelib = GestureLibraries.fromFile(path);
	  //gesturesText = (TextView)findViewById(R.id.textView1);
	  // Carga la biblioteca de gestos
	  if (!gesturelib.load()) 
	  {
		  Log.w("MainActivity", "No se puede cargar la Lista de Gestos");
		  Toast.makeText(getApplicationContext(),
                "No se puede cargar la Lista de Gestos", Toast.LENGTH_SHORT).show();
		  finish();
	  }
	  //Define la capa de Gestos
	  GestureOverlayView gestureview = (GestureOverlayView)findViewById(R.id.gesture_view);
	  gestureview.setGestureStrokeType(GestureOverlayView.GESTURE_STROKE_TYPE_MULTIPLE);

	  // Define los Listener para cada accion
	  gestureview.addOnGesturePerformedListener(gesturelistener);
  }
    // Crea el Listener de Gestos
    private GestureOverlayView.OnGesturePerformedListener gesturelistener = new GestureOverlayView.OnGesturePerformedListener() {
        @Override
        // Detecta la realizacion de un gesto en la Vista de Gestos
        public void onGesturePerformed(GestureOverlayView overlay, Gesture gesture) {
            // Almacena una lista con las posibles soluciones y la puntuacion de cada una (ordenada de mayor a menor puntuacion)
            ArrayList<Prediction> predictions = gesturelib.recognize(gesture);
            // Si obtengo una solución decente con una puntuación mayor que 4 es que hay algún patron que tiene bastante parecido al dibujado
            /*gesturesText.setTextSize(12);
            gesturesText.setText("Gestos:\n");
            for(Prediction prediction : predictions)
              gesturesText.append("N: " + prediction.name + ", P: " + prediction.score +", S: "+gesture.getStrokesCount()+"\n");*/
            if (predictions.get(0).score > 3) {
                Boolean instalado = false;
                String appName;
                Intent LaunchIntent;
                // Lista de aplicaciones del dispositivo
                List<ApplicationInfo> apps = getPackageManager().getInstalledApplications(0);
                // Comprobamos cual es el patrón dibujado
                if(predictions.get(0).name.contains("F"))
                    appName="com.facebook.katana";
                else
                    if(predictions.get(0).name.contains("W"))
                        appName="com.whatsapp";
                    else
                    if(predictions.get(0).name.contains("t"))
                        appName="com.twitter.android";
                    else
                        appName="Nothing";
                if(appName!="Nothing") {
                    // Si se ha detectado un patrón buscamos en la lista a que aplicación corresponde
                    for (ApplicationInfo app : apps) {
                        // Si la aplicación aparece significa que esta instalada, entonces la abrimos
                        if (app.className != null && app.className.contains(appName)) {
                            //gesturesText.append("Name: " + app.className + "\n");
                            instalado = true;
                            LaunchIntent = getPackageManager().getLaunchIntentForPackage(appName);
                            startActivity(LaunchIntent);
                        }
                    } // Si no esta instalada la aplicacion le mandamos al market para que se la descargue
                    if(!instalado){
                        LaunchIntent = new Intent(Intent.ACTION_VIEW);
                        LaunchIntent.setData(Uri.parse("market://details?id="+appName));
                        startActivity(LaunchIntent);
                    }
                }

            }

        }
    };

  @Override
  public boolean onCreateOptionsMenu(Menu menu) {
    /* getMenuInflater().inflate(R.menu.activity_main, menu);
    return true; */

	  return true;
  }

  /*Camera
  static final int REQUEST_IMAGE_CAPTURE = 1;
  protected void onActivityResult(int requestCode, int resultCode, Intent data) {
      if (requestCode == REQUEST_IMAGE_CAPTURE /*&& resultCode == RESULT_OK* /) {
          Bundle extras = data.getExtras();
          Bitmap imageBitmap = (Bitmap) extras.get("data");
          ImageView imageView = (ImageView) findViewById(R.id.imageView);
          imageView.setImageBitmap(imageBitmap);
      }
  } */

}
