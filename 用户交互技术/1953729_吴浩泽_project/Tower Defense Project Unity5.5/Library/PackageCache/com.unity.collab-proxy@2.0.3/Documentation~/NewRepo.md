# Getting started with a new version control repository

**Note**: To start from an existing version control repository, see [Getting started with an existing version control repository](ExistingRepo.md).

You can walk through a straightforward onboarding wizard when creating a repository for your Unity project. This new wizard will help you:

* Set up your account and configure your repository for your Unity project, enabling you to sync to a version control Cloud Edition repository.
* Generate a standard ignore file that prevents unnecessary components of your Unity project from being checked in.
* Automatically do the first check-in so that your repository is in sync with your local changes.

1. Open your Unity project.
2. To access the version control window in the Unity Editor, click **Window** &gt; **version control**:
   ![version control window](images/AccessingPlastic.png)

3. In the version control onboarding window, complete the steps to continue:
   ![Onboarding](images/Onboarding.png)

Unity connects your project to your version control Cloud repository; version control automatically creates an ignore file in the workspace for Unity projects so it doesn't track files that shouldn't be part of the repository. It also creates a standard automatic checkin during the initial setup. So now you're all set to start using version control!

![Automatic setup](images/AutomaticSetup.png)

**Note**: Basic version control actions, such as viewing pending changes, checking in changes, and viewing changesets, don’t require a version control Client install. However, if you want to use more advanced features, such as branching and diffing changeset, you will be prompted to download the version control client (if you have not already done so):

![Advanced features](images/AdvancedFeatures.png)