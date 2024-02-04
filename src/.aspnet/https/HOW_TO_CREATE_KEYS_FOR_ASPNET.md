# Creating keys for ASP.NET is tricky, but you can do it with a simple command.

1. Firstly, you should acquire a certificate from a certificate authority, or create a self-signed certificate.
2. After you have the certificate, you can create a private key and a public key with the following command:

```bash
openssl pkcs12 -export -out cert.pfx -in certificate.pem -inkey key.pem
```
> Note: replace the domain with your domain.