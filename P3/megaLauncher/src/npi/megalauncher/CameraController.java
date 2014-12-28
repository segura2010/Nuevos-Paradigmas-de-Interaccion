package npi.megalauncher;

import java.text.SimpleDateFormat;
import java.util.Date;

import android.app.Activity;
import android.content.ContentValues;
import android.content.Context;
import android.content.Intent;
import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.net.Uri;
import android.provider.MediaStore;
import android.view.View;
import android.widget.Button;
import android.widget.ImageView;
import android.widget.Toast;
import android.widget.VideoView;

public class CameraController {
	
	private static final int CAMERA_CAPTURE_IMAGE_REQUEST_CODE = 100;
	  private static final int CAMERA_CAPTURE_VIDEO_REQUEST_CODE = 200;
	  public static final int MEDIA_TYPE_IMAGE = 1;
	  public static final int MEDIA_TYPE_VIDEO = 2;

	  //Nombre de directorio de almacenamiento de imagenes y videos
	  private static final String IMAGE_DIRECTORY_NAME = "Hello Camera";

	  private Uri fileUri;
	  
	  private Context c;
	  
	  public CameraController(Context cont)
	  {
		  c = cont;
	  }
	
	   /*
	   * Capturing Camera Image 
	   */
	  public void captureImage() {
	  	// Dar un nombre a la fotograf�a para almacenarla en la carpeta por defecto del movil
	      String timeStamp = new SimpleDateFormat("yyyyMMdd_HHmmss").format(new Date());
	  	
	  	ContentValues values = new ContentValues();
	      values.put(MediaStore.Images.Media.TITLE, "IMG_" + timeStamp + ".jpg");

	      Intent intent = new Intent(MediaStore.ACTION_IMAGE_CAPTURE);
	   
	      //fileUri = ((Activity)c).getOutputMediaFileUri(MEDIA_TYPE_IMAGE);
	      fileUri = ((Activity)c).getContentResolver().insert(MediaStore.Images.Media.EXTERNAL_CONTENT_URI, values); // store content values
	      
	      intent.putExtra(MediaStore.EXTRA_OUTPUT, fileUri);
	   
	      // start el intent" de captura
	      ((Activity)c).startActivityForResult(intent, CAMERA_CAPTURE_IMAGE_REQUEST_CODE);
	  }
	  
	  /*
	   * Recording video
	   */
	  public void recordVideo() {
	  	
	      Intent intent = new Intent(MediaStore.ACTION_VIDEO_CAPTURE);
	   
	      //fileUri = getOutputMediaFileUri(MEDIA_TYPE_VIDEO);
	   
	      // set video quality
	      intent.putExtra(MediaStore.EXTRA_VIDEO_QUALITY, 1);
	   
	      //intent.putExtra(MediaStore.EXTRA_OUTPUT, fileUri);
	   
	      // start el intent" de captura de v�deo
	      ((Activity)c).startActivityForResult(intent, CAMERA_CAPTURE_VIDEO_REQUEST_CODE);
	  }
	  
	  /**
	   * Receiving activity result after closing the camera
	   * */
	  protected void onActivityResult(int requestCode, int resultCode, Intent data) {
	      //Comprobar la solicitud del usuario de guardar la fotograf�a
	      if (requestCode == CAMERA_CAPTURE_IMAGE_REQUEST_CODE) {
	    	  Toast.makeText(((Activity)c).getApplicationContext(),
	                  "CAMERA_CAPTURE_IMAGE_REQUEST_CODE", Toast.LENGTH_SHORT)
	                  .show();
	          if (resultCode == ((Activity)c).RESULT_OK)
	          {
	        	  Toast.makeText(((Activity)c).getApplicationContext(),
	                      "RESULT_OK", Toast.LENGTH_SHORT)
	                      .show();
	              previewCapturedImage();
	          } else if (resultCode == ((Activity)c).RESULT_CANCELED) {
	              Toast.makeText(((Activity)c).getApplicationContext(),
	                      "User cancelled image capture", Toast.LENGTH_SHORT)
	                      .show();
	          } else {
	              Toast.makeText(((Activity)c).getApplicationContext(),
	                      "Sorry! Failed to capture image", Toast.LENGTH_SHORT)
	                      .show();
	          }
	      }
	      else
	      {
	    	  Toast.makeText(((Activity)c).getApplicationContext(),
	                  "NO :::: CAMERA_CAPTURE_IMAGE_REQUEST_CODE", Toast.LENGTH_SHORT)
	                  .show();
	      }
	  }
	  
	  /*
	   * Display image from a path to ImageView
	   */
	  private void previewCapturedImage() {
	      try {
	          // hide video preview
	          //videoPreview.setVisibility(View.GONE);

	          //imgPreview.setVisibility(View.VISIBLE);

	          // bimatp factory
	          BitmapFactory.Options options = new BitmapFactory.Options();

	          // downsizing image as it throws OutOfMemory Exception for larger
	          // images
	          options.inSampleSize = 8;

	          Bitmap bitmap = BitmapFactory.decodeFile(fileUri.getPath(),
	                  options);
	          
	          System.out.println(fileUri.getPath());

	          //imgPreview.setImageBitmap(bitmap);
	          
	      } catch (NullPointerException e) {
	          e.printStackTrace();
	      }
	  }
	  

}
