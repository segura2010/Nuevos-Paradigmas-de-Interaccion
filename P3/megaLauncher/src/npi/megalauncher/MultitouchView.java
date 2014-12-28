package npi.megalauncher;

import java.text.SimpleDateFormat;
import java.util.ArrayList;
import java.util.Date;


import android.R.bool;
import android.app.Activity;
import android.content.ContentValues;
import android.content.Context;
import android.content.Intent;
import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.graphics.Canvas;
import android.graphics.Color;
import android.graphics.Paint;
import android.graphics.PointF;
import android.net.Uri;
import android.os.Bundle;
import android.provider.MediaStore;
import android.util.AttributeSet;
import android.util.SparseArray;
import android.view.MotionEvent;
import android.view.View;
import android.widget.Button;
import android.widget.EditText;
import android.widget.ImageView;
import android.widget.TextView;
import android.widget.Toast;
import android.widget.VideoView;

public class MultitouchView extends View {

  private static final int SIZE = 60;

  private SparseArray<PointF> mActivePointers; // Almacenara los dedos que estan en contacto con la pantalla en cada instante de tiempo
  
  // Variables necesarias para pintar los circulitos de los dedos
  private Paint mPaint;
  private int[] colors = { Color.BLUE, Color.GREEN, Color.MAGENTA,
      Color.BLACK, Color.CYAN, Color.GRAY, Color.RED, Color.DKGRAY,
      Color.LTGRAY, Color.YELLOW };
  
  private Paint textPaint;
  
  // Los siguientes arrays contendran informacion para cada dedo que este en contacto con la pantalla
  private SparseArray<Boolean> detectedFingerMoveUp; // Indicara si se ha detectado, para cada dedo, un movimiento hacia arriba
  private SparseArray<Boolean> detectedFingerMoveDown; // Indicara si se ha detectado, para cada dedo, un movimiento hacia abajo
  private SparseArray<PointF> initialPos; // Guardara la posicion inicial de cada dedo, para poder comparar y saber si se ha realizado desplazamiento
  
  
  // Necesitamos el contexto para hacer llamadas a diferentes metodos importantes de la API de Android
  Context c;
  
  // Necesitaremos un objeto de la clase encargada de hacer las llamadas a la camara
  CameraController cam;


  public MultitouchView(Context context, AttributeSet attrs) {
	  // Inicializamos el objeto
    super(context, attrs);
    c = context;
    // La clase de la camara tambien necesita el contexto para llamar a la camara
    cam = new CameraController(c);
    initView();
  }

  private void initView() {
    mActivePointers = new SparseArray<PointF>();
    
    mPaint = new Paint(Paint.ANTI_ALIAS_FLAG);
    
    // Inicializamos todo lo necesario para pintar los circulitos de la pantalla
    mPaint.setColor(Color.BLUE);
    mPaint.setStyle(Paint.Style.FILL_AND_STROKE);
    textPaint = new Paint(Paint.ANTI_ALIAS_FLAG);
    textPaint.setTextSize(20);
    
    detectedFingerMoveUp = new SparseArray<Boolean>();
    detectedFingerMoveDown = new SparseArray<Boolean>();
    initialPos = new SparseArray<PointF>();
    
    //imgPreview = new ImageView(c);
  }

  @Override
  public boolean onTouchEvent(MotionEvent event) {

    // Obtemos el identificador del dedo que ha generado el evento
    int pointerIndex = event.getActionIndex();
    int pointerId = event.getPointerId(pointerIndex);

    // Obtenemos la accion concreta que ha ocurrido
    int maskedAction = event.getActionMasked();

    // Procesamos la accion para elegir que hacer
    switch (maskedAction) {

    // Si lo que ha ocurrido es que un nuevo dedo a "pulsado" la pantalla..
    case MotionEvent.ACTION_DOWN:
    case MotionEvent.ACTION_POINTER_DOWN: {
      // Añadimos la informacion a nuestros arrays
      PointF f = new PointF();
      f.x = event.getX(pointerIndex);
      f.y = event.getY(pointerIndex);
      mActivePointers.put(pointerId, f);
      initialPos.put(pointerId, f);
      detectedFingerMoveUp.put(pointerId, false);
      detectedFingerMoveDown.put(pointerId, false);
      break;
    }
    // Si un dedo se ha movido..
    case MotionEvent.ACTION_MOVE: {
      for (int size = event.getPointerCount(), i = 0; i < size; i++) {
    	// Para cada dedo, lo buscamos en nuestros arrays y actualizamos la informacion
        PointF point = mActivePointers.get(event.getPointerId(i));
        PointF initialPoint = initialPos.get(event.getPointerId(i));
        if (point != null) {
          
        	// Realizamos la diferencia para saber el desplazamiento que se ha realizado
        	float moveX = initialPoint.x - event.getX(i);
            float moveY = initialPoint.y - event.getY(i);
            
            
            System.out.println("("+initialPoint.x+", "+initialPoint.y+") ("+event.getX(i)+", "+event.getY(i)+") (" +moveX+", "+moveY+")");
            
            // Si nos movemos 20 unidades hacia arriba o hacia abajo tendremos que hemos realizado el gesto
            if( moveY < -20 ) 
            {	// Only down move!
            	detectedFingerMoveDown.setValueAt(event.getPointerId(i), true);
            }
            else if( moveY > 20 ) 
            {	// Only up move!
            	detectedFingerMoveUp.setValueAt(event.getPointerId(i), true);
            }
          
        }
      }
      break;
    }
    // Si el dedo deja de estar en contacto con la pantalla..
    case MotionEvent.ACTION_UP:
    case MotionEvent.ACTION_POINTER_UP:
    case MotionEvent.ACTION_CANCEL: {
    	// Eliminamos la informacion del dedo de nuestros arrays
      mActivePointers.remove(pointerId);
      detectedFingerMoveUp.remove(pointerId);
      detectedFingerMoveDown.remove(pointerId);
      initialPos.remove(pointerId);
      break;
    }
    }
    invalidate();

    return true;
  }

  // Preparamos una funcion que compruebe si hemos realizado un  movimiento con dos dedos hacia arriba
  Boolean detectedUp()
  {
	  return ( detectedFingerMoveUp.size() == 2 && detectedFingerMoveUp.valueAt(0) && detectedFingerMoveUp.valueAt(1) );
  }
  
  // Preparamos una funcion que compruebe si hemos realizado un  movimiento con dos dedos hacia abajo
  Boolean detectedDown()
  {
	  return ( detectedFingerMoveDown.size() == 2 && detectedFingerMoveDown.valueAt(0) && detectedFingerMoveDown.valueAt(1) );
  }
  
  // Damos la opcion de resetear todo.. Esta funcion debe llamarse solamente en ciertas situaciones..
  // Pues perderemos informacion
  private void resetDetection()
  {
	  detectedFingerMoveDown.clear();
	  detectedFingerMoveUp.clear();
  }
  
  
  // Ahora prearamos la funcion que dibujara en pantalla los dedos detectados para que quede mas bonito
  @Override
  protected void onDraw(Canvas canvas) {
    super.onDraw(canvas);

    // Dibajamos todos los dedos que tengamos guardados (que hay pulsando) de un color determinado
    for (int size = mActivePointers.size(), i = 0; i < size; i++) {
      PointF point = mActivePointers.valueAt(i);
      if (point != null)
        mPaint.setColor(colors[i % 9]);
      canvas.drawCircle(point.x, point.y, SIZE, mPaint);
    }
    
    // Si detecto movimiento hacia arriba, aviso y llamo a la clase encargada de la camara
    if( detectedUp() )
    {	// Two fingers move detection
    	canvas.drawText("Detected Up!", 10, 40 , textPaint);
    	Toast.makeText(((Activity)c).getApplicationContext(),
                "Image Detected! (Up)", Toast.LENGTH_SHORT)
                .show();
    	resetDetection(); // Reseteo la deteccion para evitar que en el mismo intante habra mas veces la camara
    	cam.captureImage();
    }
    // Si detecto movimiento hacia abajo, aviso y llamo a la clase encargada de la camara
    else if( detectedDown() )
    {
    	canvas.drawText("Detected Down!", 10, 40 , textPaint);
    	Toast.makeText(((Activity)c).getApplicationContext(),
                "Video Detected! (Down)", Toast.LENGTH_SHORT)
                .show();
    	resetDetection(); // Reseteo la deteccion para evitar que en el mismo intante habra mas veces la camara
    	cam.recordVideo();
    }
    
    //imgPreview = (ImageView) findViewById(R.id.imgPreview);
    
  }
  


 
  
} 