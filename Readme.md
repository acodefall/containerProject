1) Download the code to /dev/docker

2) Create a Bridge Network in Docker.
		Name of the subnet could be anything(Eg: Rnet2).
		
                docker network create --subnet=172.22.0.0/16 Rnet2
	
3) Create Container to run RabbitMQ, and launch it.
		
                >cd /dev/docker/RabbitAsContainerLB/RabbitMQ
	        >docker build -t dems:RabbitMQx1 .
         	>docker run --network Rnet2 -h RabbitMQx1host --ip 172.22.0.3 -it -p 1883:1883 -p 15673:15672 --rm dems:RabbitMQx1
		
4) Create a User in RabbitMQ.
		User should have 'AdminRights'. Try to log-in as that user for testing.
		
		http://localhost:15673
                  u: test
		  p: test

5) These config files will be mapped to container
		
                /dev/docker/RabbitAsContainerLB/ComsumerHashAPI/src/ComsumerHashAPI/Config/ConsumerNormal.txt
		
                /dev/docker/RabbitAsContainerLB/ProduceToQAPI/src/ProduceToQAPI/Config/PublishToQ.txt
	
6) Build Docker image for ComsumerHashAPI
		
                >cd "/dev/docker/RabbitAsContainerLB/ComsumerHashAPI/src/ComsumerHashAPI"
		>dotnet build -c release -r centos.7-x64
		>dotnet publish -c release -r centos.7-x64
		>docker build -t dems:ComsumerHashAPI .
	
7) Bring up two instances of ComsumerHashAPI	
		
                >cd "/dev/docker/RabbitAsContainerLB/ComsumerHashAPI/src/ComsumerHashAPI"
                
		>docker run  --rm -h ComsumerHashAPIHost1 --network Rnet2 --ip 172.22.0.13 -it -v /dev/docker/RabbitAsContainerLB/ComsumerHashAPI/src/ComsumerHashAPI/Config:/root/appconfig:rw -p 8081:6000 dems:ComsumerHashAPI 
                
		>docker run  --rm -h ComsumerHashAPIHost2 --network Rnet2 --ip 172.22.0.14 -it -v /dev/docker/RabbitAsContainerLB/ComsumerHashAPI/src/ComsumerHashAPI/Config:/root/appconfig:rw -p 8082:6000 dems:ComsumerHashAPI

8) Build Docker image for ProduceToQAPI
		
                >cd "/dev/docker/RabbitAsContainerLB/ProduceToQAPI/src/ProduceToQAPI"
		>dotnet build -c release -r centos.7-x64
		>dotnet publish -c release -r centos.7-x64
		>docker build -t dems:ProduceToQAPI .
		

9) Bring up one instance of ProduceToQAPI	
		
                >cd "/dev/docker/RabbitAsContainerLB/ProduceToQAPI/src/ProduceToQAPI"
		>docker build -t dems:ProduceToQAPI .
		>docker run -h ProduceToQAPIHost --network Rnet2  --ip 172.22.0.15 -it -v /dev/docker/RabbitAsContainerLB/ProduceToQAPI/src/ProduceToQAPI/Config:/root/appconfig:rw  -p 8090:5000 --rm dems:ProduceToQAPI
				
10)	Start consumer and producers
		
		localhost:8081/api/Consume1/Start
		localhost:8082/api/Consume1/Start
		localhost:8090/api/PostToQ/Start
					
		
		
