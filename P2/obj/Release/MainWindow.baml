   M S B A M L   `   `   `  ����   SkeletonBasics-WPF8    2Microsoft.Samples.Kinect.SkeletonBasics.MainWindow   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     -     � Title$  Skeleton Basics��$	��735��$	��770�� � 	Resources    MediumGreyBrush �� Color  KinectPurpleBrush  KinectBlueBrush �� 
TargetType �� Property �� Value  SquareCheckBox �� 
TargetType �� Orientation �Name	 ��Name
 ��Name �� TextAlignment �� Triggers P� Property P� Value �� 
TargetName��%�  &	        &	      &	 *     '�� ?     &	 t     7  7  �� $ 	#ff6e6e6e���� $ 	#ff52318f���� $ 	#ff00BCF2���� " ��.���  �@J�SnapsToDevicePixels . �� " ��.���  � �� �� " ��.R�� .+��� $ 
Horizontal=�
������ .+�� $ SquareCheckBoxChecked��.+��� $	 CheckedNormal��$��Images\CheckedNormal.png��$
��None=�$��Center=��� $	 CheckedHover��$��Images\CheckedHover.png��$
��None=�$��Center=�$y�	Collapsed=�� $ SquareCheckBoxUnchecked��$y�	Collapsed=�.+��� $	 UncheckedNormal��$ ��Images\UncheckedNormal.png��$
��None=�$��Center=��� $	 UncheckedHover��$��Images\UncheckedHover.png��$
��None=�$��Center=�$y�	Collapsed=��� $
 SquareCheckBoxText��#��z��$
 Left=�$��Center=�8��  $��15(�$��9,0,0,0q� P�  �@d�	IsChecked$ falseY�. ��� $ SquareCheckBoxChecked�� � y�$ 	Collapsed=��� $ SquareCheckBoxUnchecked�� � y�$ Visible=�P�  �@J�IsMouseOver . . ��� $ CheckedNormal�� � y�$ 	Collapsed=��� $ UncheckedNormal�� � y�$ 	Collapsed=��� $ CheckedHover�� � y�$ Visible=��� $ UncheckedHover�� � y�$ Visible=��� 8  $ SquareCheckBoxText�� � ��.��� -   $��
layoutGrid��$��	10 0 10 0q� � RowDefinitions �� $
��Auto ��� $��* ��� $
��Auto ��� $
��Auto �.+��� -   $��Stats��$��3,100,3.333,452.667q�
�������$��2��._��� -   $��resultStats��$��20(�$��0,111,-0.667,-111q���Bienvenido al juego de ejercicio de Barcenas. En este juego debereas realizar dos ejercicios uno detrás de otro durante 1 minuto. El objetivo es recoger el mayor número de monedas. Las monedas indican por donde debes pasar tus manos para realizar el movimiento, pero ten cuidado! Si no lo haces bien se te caerán las monedas y tendrás que volver a empezar el ejercicio. Al final del juego podrás ver cuantas monedas has conseguido, y además, el señor Barcenas te dará algo más de dinero dependiendo de lo que le hayas gustado. VAMOS! NO DEJES TU SOBRE VACIO!]� $��0��$��0,0,30.333,65q�.+��� $��Right=�$��
0,10,-1,10q�#��[  $��Segoe UI)�$��18(�$	��493����  � Language$ es-es�$ �Haz Ejercicio con Barcenas��(� -   $��ImageV��$y�Hidden=�$��1�� (� Stretch$ Uniform=�$��Center=�.���� -   $��Image��$	��640��$	��480��(� -   $��Image2V��$y�Hidden=�$��1��$ Uniform=�$��Center=�.���� -   $��Image2��$	��640��$	��480���� -	   $��	statusBar��$��3��$��Stretch=�$��Bottom=�
�������#��[  .@��� -
   $
 statusBarText��$��90 0 0 0q�$��20(�$��Right=��� $ es-es�$�Status Kinect: OK!���� -   $��startButton��$��3,423,-3.667,9.667q�$��1��.��Iniciar Juego�� -   $��medal��$y�Hidden=�$��3,423,-3.667,9.667q�$��1���� -   $	 GuiaEsqueleto��$y�Hidden=�$
��Left=�$	��168��$	��Top=�$	��260��$��496,-6,-14,0q� J� IsManipulationEnabled . $��2���� -   $	 imagen_Resultado��$y�Hidden=�$
��Left=�$	��180��$��214,243,0,0q�$��1��$	��Top=�$	��320�� . �� -   $	 icono_inicio��$/��)Project;component/Images/iconBarcenas.png��$y�Visible=�$
��Left=�$��68��$��	10,10,0,0q�$	��Top=�$��69��