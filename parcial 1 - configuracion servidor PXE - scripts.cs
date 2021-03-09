

INSTALAR SERVICIOS
yum install vim net-tools nano wget unzip
yum install xinetd syslinux tftp-server
yum install dhcp tftp vsftpd

PAGINA CONSULTA DE APOYO CONFIGURACION SERVIDOR PXE
https://www.linuxparty.es/29-internet/10072-configure-el-servidor-de-instalacion-pxe-arranque-de-red-en-centos-7-x

¿Qué es Xinetd?
Se trata de un demonio (daemon, en inglés) y super servicio TCP-wrapped (Envoltorio de TCP) que controla el acceso 
a un subconjunto de servicios de red populares, incluyendo FTP, IMAP y Telnet. También proporciona opciones de 
configuración específicas del servicio para control de acceso, registro mejorado, vinculación, redirección y control 
de utilización de recursos. Su nombre viene del acrónimo en inglés “eXtended InterNET Daemon”, esto es, Demonio Extendido
 de Internet. Es típico de los sistemas UNIX y like-UNIX. También se considera una extensión mucho más segura del servicio
de Internet inetd.

¿Cómo funciona?
Cuando un cliente intenta conectar con un servicio de red controlado por xinetd, el super servicio recibe la petición 
y chequea sus reglas de control de acceso. Si se permite el acceso, el demonio verifica que la conexión está permitida 
utilizando sus propias de reglas de acceso para ése servicio. Además comprueba que el servicio es capaz de tener más 
recursos asignados a él y que no está en conflicto con las reglas definidas.

Si estas condiciones se cumplen, esto es, se permite el acceso al servicio, dicho servicio no ha alcanzado su límite de 
recursos y no se encuentra en incumplimiento de cualquier regla definiida, xinetd inicia una instancia del servicio 
solicitado. Una vez establecida la conexión entre el cliente y el servicio el demonio deja de participar.

Configuración y funcionamiento
El fichero principal de configuración del demonio es /etc/xinetd.conf y el directorio que contiene los ficheros específicos
de los servicios es /etc/xinetd.d/

El archivo /etc/xinetd.conf contiene información general sobre el demonio. Cada vez que modificamos algo de su 
configuración debemos reiniciarlo.

https://www.ochobitshacenunbyte.com/2017/01/18/servicios-de-internet-en-linux-con-xinetd/

Syslinux 
Es un paquete constituido por una familia de cargadores de arranque. El paquete incluye SYSLINUX (para sistemas de
ficheros FAT), EXTLINUX (para sistemas de ficheros ext2/3/4, btrfs y xfs), PXELINUX (para arrancar desde la red con PXE) 
y ISOLINUX (ISO-9660) para arrancar desde CD/DVD.
https://wiki.gentoo.org/wiki/Syslinux/es


ARCHIVO ISO CENTOS LINUX
http://mirror.cedia.org.ec/centos/7.9.2009/isos/x86_64/CentOS-7-x86_64-DVD-2009.iso


El servidor PXE (Preboot eXecution Environment) permite la instalación desatendida o automatizada del sistema operativo 
a través de la red. La principal ventaja de pxe es que no necesitamos ninguna unidad de arranque para iniciar el sistema 
operativo (sistema operativo) y no necesitamos grabar ningún archivo ISO en DVD o dispositivo USB.

Una vez configurado el servidor PXE, podemos instalar cientos de sistemas al mismo tiempo a través de la red. Como funciona
en la arquitectura cliente-servidor, para obtener la instalación del sistema operativo en los clientes, inicie los clientes
a través de la opción PXE.

Paso: 1 Instala los paquetes requeridos para la configuración de PXE
Para instalar y configurar el servidor pxe en centos 7.x necesitamos los siguientes paquetes 
"dhcp, tftp-server, ftp server (vsftpd), xinted".
[root@pxe ~]# yum install dhcp tftp tftp-server syslinux vsftpd xinetd

Paso: 2 Configurar el servidor DHCP para PXE
Cuando instalamos el paquete dhcp, se crea un archivo de configuración de muestra del servidor dhcp en 
" /usr/share/doc/dhcp*/dhcpd.conf.example ", aunque el archivo de configuración de dhcp está en ' / etc / dhcp / dhcpd. conf '.

[root@servidorPXE ~]# vim /etc/dhcp/dhcpd.conf

# DHCP Server Configuration file.

ddns-update-style interim;
ignore client-updates;
authoritative;
allow booting;
allow bootp;
allow unknown-clients;

# internal subnet for my DHCP Server
subnet 192.168.50.0 netmask 255.255.255.0 {
range 192.168.50.21 192.168.50.151;
option domain-name-servers 192.168.50.3;
option domain-name "pxe.example.com";
option routers 192.168.50.3;
option broadcast-address 192.168.50.255;
default-lease-time 600;
max-lease-time 7200;

# IP of PXE Server
next-server 192.168.50.10;
filename "pxelinux.0";
}

Paso: 3 Editar y configurar el servidor tftp (/etc/xinetd.d/tftp)
TFTP (Protocolo de transferencia de archivos trivial) se utiliza para transferir archivos desde el servidor de datos a sus 
clientes sin ningún tipo de autenticación. En el caso de la configuración del servidor PXE, tftp se utiliza para cargar 
bootstrap. Para configurar el servidor tftp, edite su archivo de configuración ' /etc/xinetd.d/tftp ', 
cambie el parámetro ' disable = yes ' a ' disable = no' y deje los demás parámetros como están.

[root@servidorPXE ~]# vim /etc/dhcp/dhcpd.conf
service tftp
{
        socket_type             = dgram
        protocol                = udp
        wait                    = yes
        user                    = root
        server                  = /usr/sbin/in.tftpd
        server_args             = -s /var/lib/tftpboot
        disable                 = no
        per_source              = 11
        cps                     = 100 2
        flags                   = IPv4
}

Todos los archivos relacionados con el arranque de red se colocarán en el directorio raíz tftp “/ var / lib / tftpboot ”
Ejecute los siguientes comandos para copiar los archivos de arranque de red necesarios en '/ var / lib / tftpboot /'

[root@servidorPXE ~]# cp -v /usr/share/syslinux/pxelinux.0 /var/lib/tftpboot
[root@servidorPXE ~]# cp -v /usr/share/syslinux/menu.c32 /var/lib/tftpboot
[root@servidorPXE ~]# cp -v /usr/share/syslinux/memdisk /var/lib/tftpboot
[root@servidorPXE ~]# cp -v /usr/share/syslinux/mboot.c32 /var/lib/tftpboot
[root@servidorPXE ~]# cp -v /usr/share/syslinux/chain.c32 /var/lib/tftpboot
[root@servidorPXE ~]#
[root@servidorPXE ~]# mkdir /var/lib/tftpboot/pxelinux.cfg
[root@servidorPXE ~]# mkdir /var/lib/tftpboot/networkboot

Paso: 4 Monte el archivo ISO CentOS 7.x y copie su contenido al servidor ftp local
Ejecute los comandos siguientes para montar el archivo iso y luego copie su contenido en el directorio del servidor ftp
 ' / var / ftp / pub '

[root@servidorPXE ~]# mkdir mnt
[root@servidorPXE ~]# mount -o loop mount -o loop /var/ftp/pub/CentOS-7-x86_64-DVD-2009.iso /mnt/
mount: /dev/loop0 is write-protected, mounting read-only
[root@servidorPXE ~]# cd /mnt/
[root@servidorPXE ~]# cp -av * /var/ftp/pub/

Copie el archivo Kernel (vmlimz) y el archivo initrd del archivo iso montado a ' / var / lib / tftpboot / networkboot / '

[root@servidorPXE ~]# cp /mnt/images/pxeboot/vmlinuz /var/lib/tftpboot/networkboot/
[root@servidorPXE ~]# cp /mnt/images/pxeboot/initrd.img /var/lib/tftpboot/networkboot/

Ahora puede desmontar el archivo iso usando el comando 'umount'
[root@servidorPXE ~]# umount /mnt/

Paso: 5 Crea el archivo de menú kickStart & PXE.
Antes de crear el archivo kickstart, primero creemos la contraseña de root en una cadena cifrada porque usaremos esa cadena de 
contraseña cifrada en el archivo kickstart.

[root@servidorPXE ~]# openssl passwd -1 Pxe@123#
$1$e2wrcGGX$tZPQKPsXVhNmbiGg53MN41

El archivo kickstart predeterminado del sistema se coloca en / root con el nombre ' anaconda-ks.cfg '. Crearemos un nuevo 
kickstart en la carpeta / var / ftp / pub con el nombre ' centos7.cfg ' Copia el siguiente contenido en el nuevo archivo kickstart.

[root@servidorPXE ~]# vim /var/ftp/pub/centos7.cfg

#platform=x86, AMD64, or Intel EM64T
#version=DEVEL
# Firewall configuration
firewall --disabled
# Install OS instead of upgrade
install
# Use FTP installation media
url --url="ftp://192.168.50.10/pub/"
# Root password
rootpw --iscrypted $1$e2wrcGGX$tZPQKPsXVhNmbiGg53MN41
# System authorization information
auth useshadow passalgo=sha512
# Use graphical install
graphical
firstboot disable
# System keyboard
keyboard us
# System language
lang en_US
# SELinux configuration
selinux disabled
# Installation logging level
logging level=info
# System timezone
timezone Europe/Amsterdam
# System bootloader configuration
bootloader location=mbr
clearpart --all --initlabel
part swap --asprimary --fstype="swap" --size=1024
part /boot --fstype xfs --size=300
part pv.01 --size=1 --grow
volgroup root_vg01 pv.01
logvol / --fstype xfs --name=lv_01 --vgname=root_vg01 --size=1 --grow
%packages
@^minimal
@core
%end
%addon com_redhat_kdump --disable --reserve-mb='auto'
%end

Cree un archivo de menú PXE ( /var/lib/tftpboot/pxelinux.cfg/default ), copie los siguientes contenidos en el archivo de menú pxe

[root@servidorPXE ~]# vim /var/lib/tftpboot/pxelinux.cfg/default

default menu.c32
prompt 0
timeout 30
MENU TITLE LinuxTechi.com PXE Menu
LABEL centos7_x64
MENU LABEL CentOS 7_X64
KERNEL /networkboot/vmlinuz
APPEND initrd=/networkboot/initrd.img inst.repo=ftp://192.168.50.10/pub ks=ftp://192.168.50.10/pub/centos7.cfg

Paso: 6 Inicia y habilita los servicios xinetd, dhcp y vsftpd.
Use los siguientes comandos para iniciar y habilitar xinetd, dhcp y vsftpd.

[root@servidorPXE ~]# service xinetd start
[root@servidorPXE ~]# service dhcpd start
[root@servidorPXE ~]# service vsftpd start

En caso de que SELinux esté habilitado, configure la siguiente regla de selinux para el servidor ftp.

[root@servidorPXE ~]# setsebool -P allow_ftpd_full_access 1