set OPENSSL_CONF=c:\OpenSSL-Win64\bin\openssl.cfg
openssl genrsa -out private.key 4096
openssl req -new -x509 -key private.key -out publickey.cer -days 730 -sha512
openssl pkcs12 -export -out Certificate.pfx -inkey private.key -in publickey.cer