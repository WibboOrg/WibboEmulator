FROM debian:10 as intermediate

RUN apt-get update

RUN apt-get install -y git

RUN mkdir /root/.ssh/

COPY /root/.ssh/id_rsa /root/.ssh/id_rsa
RUN chmod 600 /root/.ssh/id_rsa
RUN touch /root/.ssh/known_hosts
RUN ssh-keyscan github.com >> /root/.ssh/known_hosts

RUN git clone git@github.com:JasonDhose/WibboEmulator.git

FROM mono:latest

COPY --from=intermediate /WibboEmulator /Emulator

RUN msbuild /p:Configuration=Release /Emulator/ButterflyEmulator.csproj

COPY /bin/configuration /Emulator/bin/configuration

WORKDIR /Emulator/bin/log

EXPOSE 444/tcp 30001/tcp 527/tcp
VOLUME /Emulator/bin

CMD [ "mono", "/Emulator/bin/ButterflyEmulator.exe" ]