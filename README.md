# Manipulador myCobot Pro 630 + Meta Quest

##Introdução:

Este aplicativo em Unity exibe um modelo digital do manipulador robótico myCobot Pro 630 em um ambiente de realidade aumentada usando o Meta Quest. Junto ao braço existe um cubo levemente translúcido que serve como referência para a posição-alvo da garra do manipulador. É possível mover o cubo para outra posição usando o movimento de pinça com as mãos. Ao fazer isso, depois de alguns instantes com o cubo estacionário, o braço robótico moverá a garra para a nova posição do cubo. Caso seja detectada alguma colisão ou posição onde a garra não seja capaz de alcançar, é exibida uma mensagem de posição inválida.

##Arquitetura:

Esta aplicação conta com um Frontend que requer uma conexão de rede com o Backend (pode ser por wifi ou 4G). O requisito é que o aplicativo seja capaz de se comunicar com o broker MQTT, e que o broker MQTT seja capaz de se comunicar com o ambiente ROS2. O aplicativo Unity também pode ser compilado para Windows, onde possui funcionalidades levemente diferentes devido à interface, como por exemplo, o posicionamento do cubo usando o Mouse, sliders para controle de cada junta do robô e a opção de habilitar ou desabilitar a cinemática inversa por meio do ROS2.

###Frontend
O frontend é o aplicativo Unity. Este se comunica com o backend usando o protocolo MQTT. Por padrão, o broker MQTT é o `perspex.ddns.net:1883` e os tópicos usados são `pingpong/primitiveB` para envio e `pingpong/primitive` para recebimento de dados. Isto pode ser alterado dentro do Unity no Objeto M2MQTT, alterando as variáveis `Broker Address`, `MQTT Topic Send` e `MQTT Topic Receive`.

###Backend
O backend constitui de um broker MQTT (atualmente utilizando o [Mosquitto](https://mosquitto.org/)), e do ambiente ROS2. 
O broker MQTT funciona como um intermediário entre o Frontend e o ROS2, já que o Unity (ainda) não conta com plugins que habilitem a comunicação direta com o ROS2 na plataforma Android.
O ambiente ROS2 é reponsável por receber os dados de posição-alvo enviadas pelo Unity no tópico de envio e retornar a trajetória calculada (ou mensagem de erro) no tópico de recebimento.

##Como instalar:

#Aplicação Unity:
O aplicativo foi desenvolvido em Unity 2023.2.14f1 e compilado para Android para uso no Meta Quest, ou Windows para uso em PCs.

#MQTT Broker:
Os passos para instalação do broker MQTT Mosquitto são:
1) Baixar o pacote de instalação do [Mosquitto](https://mosquitto.org/).
2) Editar o mosquitto.conf com as linhas:
```
listener 1883
allow_anonymous true
```
É possível definir usuário e senha neste arquivo, o que exigirá uma definição também de usuário e senha no Objeto M2MQTT do Unity.
3) Executar o mosquitto.exe (para Windows) ou outro executável em outros sistemas.

#ROS2
O ROS2 foi instalado no ambiente WSL (Windows 11), com um sistema Ubuntu 22.04.4. A versão ROS2 usada é o ROS2 Humble. Os arquivos do backend e os passos para a instalação e execução estão disponível no repositório [ROS2-myCobotPro](https://github.com/EMCCERISE/ROS2-myCobotPro).
