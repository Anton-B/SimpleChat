   M S B A M L   `   `   `  ����H  DSimpleChatWPF, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null-clr-namespace:SimpleChatWPFSimpleChatWPF  5   	       SimpleChatWPF.MainWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   -localclr-namespace:SimpleChatWPF     5   	     � Title$  Чат��5   	   $	��800��6   $	��600��6"   $	��600��6.    � WindowStartupLocation$ CenterScreen=�6=    � 
ResizeMode$ CanMinimize=�6b   .��5	      ]� -   5         ]�Name$ mainStackPanel��5	      $��30, 10q�6(   .+�5
   
   � -   5         �Name$ buttonsGrid��5
      $	��Top=�6$    � ColumnDefinitions 5      �� 5      $��0.82* �6#   �� 5      $��* �6#   �� 5      $��* �6#   �� 5      $��0.82* �6#   .+�5      �� -   5         ��Name$ changeNameButton��5      $��0��6/   $
��Left=�6?    � Cursor$
 Hand��6Z   $��Изменить имя��6h   |�.  $��28��6�   $��96��6�   6�   �� 5      -   5        $ createChatButton��5      $��1��6/   $
 Hand��6?   $��Создать чат��6M   |�.  $��28��6u   $��96��6�   6�   �� 5      -   5        $ joinChatButton��5      $��2��6-   $
 Hand��6=   $"��Присоединиться��6K   |�.  $��28��6v   $��96��6�   6�   �� 5      -   5        $ 
exitButton��5      $��3��6)   $��Right=�69   $
 Hand��6U   |�.  $��
Выйти��6u   $��28��6�   $��96��6�   6�   5      ]� 5   
   -   5        $ inputDockPanel��5      $��Bottom=�6,   .+�5      �� -   5         ��Name$ inputTextBox��5      $��60��6,   	 �� TextWrapping$
	 Wrap=�68   6L   5      �� 5   
   -	   5        
 ��Name$
 
chatBorder��5      
�������$��0,4,0,0q�68   $��1,1,1,0q�6I    �� CornerRadius$ 2,2,0,0��6c   ._�5      �� -
   5         ��Name$ chatScrollViewer��5      $
��Auto=�65   |�.  .��5      �� -   5         ��Name$ chatStackPanel��5      5      5      5      5      