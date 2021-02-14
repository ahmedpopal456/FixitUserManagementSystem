from locust import HttpUser, task, between, tag
import uuid

class FixitUser(HttpUser):
    host = "http://localhost:7071"
    wait_time = between(2, 5)
    user_id = ""

    @tag('get_user_profile', 'profile')
    @task(6)
    def get_user_profile(self):
        self.client.get(f"/api/{self.user_id}/account/profile",
            name="get_user_profile")

    @tag('update_user_profile', 'profile')
    @task(4)
    def update_user_profile(self):
        self.client.put(f"/api/{self.user_id}/account/profile",
            json={
                "firstName": "Performance",
                "lastName": "Testing",
                "address": {
                    "address": "124 Something",
                    "city": "Montreal",
                    "province": "Quebec",
                    "postalCode": "A1B2C0",
                    "country": "Canada",
                    "phoneNumber": "514-123-6543"
                }
            },
            name="update_user_profile")

    @tag('update_user_profile_picture', 'profile')
    @task(4)
    def update_user_profile_picture(self):
        self.client.put(f"/api/{self.user_id}/account/profile/profilePicture",
            json={"profilePictureUrl": "something.something/something2.jpg"},
            name="update_user_profile_picture")

    @tag('update_account_status', 'account')
    @task(2)
    def update_account_status(self):
        self.client.put("/api/3441a80b-cf00-41f5-80f1-b069f1d3cda6/account",
            json={"state":"enabled"},
            name="update_account_status")

    def create_user(self):
        email_id = uuid.uuid4()
        response = self.client.post("/api/me/account",
            json={
                "FirstName": "Performance",
                "LastName": "Test",
                "UserPrincipalName": f"pt{email_id}@fixitb2ctest.onmicrosoft.com",
                "Role": "client"
            },
            name="create_user")
        json_response = response.json()
        self.user_id = json_response['userId']

    def on_start(self):
        self.create_user()