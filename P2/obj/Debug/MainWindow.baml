   M S B A M L   `   `   `  ����  SkeletonBasics-WPF8    2Microsoft.Samples.Kinect.SkeletonBasics.MainWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   -   5          � Title$  Skeleton Basics��5   	   $	��735��6!   $	��770��6.    � 	Resources 5         MediumGreyBrush �� Color  KinectPurpleBrush  KinectBlueBrush �� 
TargetType �� Property �� Value  SquareCheckBox �� 
TargetType �� Orientation �Name	 ��Name
 ��Name �� TextAlignment �� Triggers P� Property P� Value �� 
TargetName��%1  &	        &	 #     &	 F     '�� i     &	 �     7  7  �� 5   
   $ 	#ff6e6e6e��62   �� 5   
   $ 	#ff52318f��64   �� 5	   
   $ 	#ff00BCF2��62   �� 5
   
   " ��6   .�5      ��  �@J�SnapsToDevicePixels . 64   5      �� 5   
   " ��6   .�5      ��  � �� 5      �� 5      " ��6&   .R�5      � .+�5      �� $ 
Horizontal=�6)   
������ .+�5   "   � $ SquareCheckBoxChecked��6'   .+�5   &   �� $	 CheckedNormal��6,   $��Images\CheckedNormal.png��6C   $
��None=�6e   $��Center=�6t   �� 5   &   $	 CheckedHover��6,   $��Images\CheckedHover.png��6B   $
��None=�6c   $��Center=�6r   $y�	Collapsed=�6�   5   #   � 5   "   $ SquareCheckBoxUnchecked��6'   $y�	Collapsed=�6H   .+�5   &   �� $	 UncheckedNormal��6,   $ ��Images\UncheckedNormal.png��6E   $
��None=�6i   $��Center=�6x   �� 5   &   $	 UncheckedHover��6,   $��Images\UncheckedHover.png��6D   $
��None=�6g   $��Center=�6v   $y�	Collapsed=�6�   5   #   �� 5   "   $
 SquareCheckBoxText��6,   #��z��6H   $
 Left=�6i   $��Center=�6   8��  $��15(�6�   $��9,0,0,0q�6�   5      5       5      P� 5       �@d�	IsChecked$ falseY�6;   . �5    "   �� $ SquareCheckBoxChecked��6Q    � y�$ 	Collapsed=�6?   6Q   �� 5!   "   $ SquareCheckBoxUnchecked��6O    � y�$ Visible=�6?   6O   5"      P� 5#       �@J�IsMouseOver . . �5$   "   �� $ CheckedNormal��6Q    � y�$ 	Collapsed=�6?   6Q   �� 5%   "   $ UncheckedNormal��6Q    � y�$ 	Collapsed=�6?   6Q   �� 5&   "   $ CheckedHover��6O    � y�$ Visible=�6?   6O   �� 5'   "   $ UncheckedHover��6O    � y�$ Visible=�6?   6O   �� 5(   "   8  $ SquareCheckBoxText��6h    � ��5)      5+      5-      5.      5/      .��51      � -   5        $��
layoutGrid��51      $��	10 0 10 0q�6    � RowDefinitions 52   
   �� 53      $
��Auto �6   �� 54      $��* �6   �� 55      $
��Auto �6   �� 56      $
��Auto �6   .+�58   
   �� -   5        $��Stats��58      $��3,83,3.333,92.667q�6   
�������$��2��6L   ._�59      �� -   5        $��resultStats��59      $��14(�6+   $��0,111,-0.667,-111q�69   ��Bienvenido al juego de ejercicio de Barcenas. En este juego debereas realizar dos ejercicios uno detrás de otro durante 1 minuto. El objetivo es recoger el mayor número de monedas. Las monedas indican por donde debes pasar tus manos para realizar el movimiento, pero ten cuidado! Si no lo haces bien se te caerán las monedas y tendrás que volver a empezar el ejercicio. Al final del juego podrás ver cuantas monedas has conseguido, y además, el señor Barcenas te dará algo más de dinero dependiendo de lo que le hayas gustado. VAMOS! NO DEJES TU SOBRE VACIO!6�  5;      ]� 5<   
   $��0��6   $��0,0,30.333,65q�6!   .+�5=      �� $��Right=�6   $��
0,10,-1,10q�6/   #��[  6C   $��Segoe UI)�6q   $��18(�6�   $	��493��6�   �� 6�    � Language$ es-es�6�   $ �Haz Ejercicio con Barcenas��6�   6�   5>      (� 5?   
   -   5        $��ImageV��5?   &   $y�Hidden=�6   $��1��64    (� Stretch$ Uniform=�6A   $��Center=�6S   .��5@      �� -   5        $��Image��5@      $	��640��6!   $	��480��6-   5A      (� 5B   
   -   5        $��Image2V��5B   &   $y�Hidden=�6   $��1��65   $ Uniform=�6B   $��Center=�6T   .��5C      �� -   5        $��Image2��5C      $	��640��6"   $	��480��6.   5D      �� 5F   
   -	   5        $��	statusBar��5F   ?   $��3��6   $��Stretch=�6!   $��Bottom=�6P   
�������#��[  6~   .@�5G      �� -
   5        $
 statusBarText��5G      $��90 0 0 0q�6/   $��20(�6A   $��Right=�6O   �� 6l   $ es-es�6p   $�Status Kinect: OK!��6�   6�   5H      �� 5J   
   -   5        $��startButton��5J      $��3,423,-3.667,9.667q�66   $��1��6R   .��6`   Iniciar Juego6o   �� 5K   
   -   5        $��medal��5K   $   $y�Hidden=�6   $��3,423,-3.667,9.667q�61   $��1��6M   �� 5M   
   -   5        $��imagen_Resultado��5M   $   $y�Hidden=�6   $
��Left=�6<   $	��180��6W   $��214,243,0,0q�6d   $��1��6y   $	��Top=�6�   $	��320��6�    J� IsManipulationEnabled . 6�   �� 5N   
   -   5        $	 icono_inicio��5N   ^   $5��/pack://siteoforigin:,,,/Images/iconBarcenas.png��6   $y�Visible=�6I   $
��Left=�6t   $��68��6�   $��	10,10,0,0q�6�   $	��Top=�6�   $��69��6�   � 5P   
   -   5        $��tutorial��5P   $   $y�Visible=�6   $��33,109,10.333,156.667q�64   $��2��6S   r� 5Q   
   -   5         r�Name$ Guia_Esqueleto��5Q      $
��Left=�6/   $	��168��6J   $��
476,72,0,0q�6W   $��2��6k   $	��Top=�6|   $	��219��6�   �� 5R   
   -   5         ��Name$ 
puntuacion��5R      $
��Left=�6&   $��48��6A   $��
174,61,0,0q�6M    �� TextWrapping$
 Wrap=�6a   $	��Top=�6u   $	��538��6�   $��20(�6�   $��Segoe UI Black)�6�   ���	#FFF9FBFB���	#FF0685236�   5U      5V      