deploy:
	{ \
	sshpass -p $(password) ssh -o StrictHostKeyChecking=no deploy@$(server) "cd /var/services/platform &&\
	docker image prune --all --force &&\
	docker-compose pull billingapi-app &&\
	docker-compose up -d --no-deps billingapi-app" ;\
	}