   M S B A M L   `   `   `  ���� H  DSimpleChatWPF, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null-clr-namespace:SimpleChatWPFSimpleChatWPF      SimpleChatWPF.MainWindow   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     -localclr-namespace:SimpleChatWPF       � Title$  Чат��$	��800��$	��600��$	��600�� � WindowStartupLocation$ CenterScreen=� � 
ResizeMode$ CanMinimize=�.��]� -    ]�Name$ mainStackPanel��$��30, 10q�.+�� -    �Name$ buttonsGrid��$	��Top=� � ColumnDefinitions �� $��0.82* ��� $��* ��� $��* ��� $��0.82* �.+��� -    ��Name$ changeNameButton��$��0��$
��Left=� � Cursor$
 Hand��$��Изменить имя��|�.  $��28��$��96���� -   $ createChatButton��$��1��$
 Hand��$��Создать чат��|�.  $��28��$��96���� -   $ joinChatButton��$��2��$
 Hand��$"��Присоединиться��|�.  $��28��$��96���� -   $ 
exitButton��$��3��$��Right=�$
 Hand��|�.  $��
Выйти��$��28��$��96��]� -   $ inputDockPanel��$��Bottom=�.+��� -    ��Name$ inputTextBox��$��60��	 �� TextWrapping$
	 Wrap=��� -	   
 ��Name$
 
chatBorder��
�������$��0,4,0,0q�$��1,1,1,0q� �� CornerRadius$ 2,2,0,0��._��� -
    ��Name$ chatScrollViewer��$
��Auto=�|�.  .���� -    ��Name$ chatStackPanel��