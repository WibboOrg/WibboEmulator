FROM mono:latest
COPY . .
RUN msbuild /p:Configuration=Release ButterflyEmulator.csproj

EXPOSE 30010 30001 527 3306

CMD [ "mono", "/bin/ButterflyEmulator.exe" ]