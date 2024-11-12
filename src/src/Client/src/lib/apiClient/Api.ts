/* eslint-disable */
/* tslint:disable */
/*
 * ---------------------------------------------------------------
 * ## THIS FILE WAS GENERATED VIA SWAGGER-TYPESCRIPT-API        ##
 * ##                                                           ##
 * ## AUTHOR: acacode                                           ##
 * ## SOURCE: https://github.com/acacode/swagger-typescript-api ##
 * ---------------------------------------------------------------
 */

import { LoginCommand, RegisterCommand, User } from "./data-contracts";
import { ContentType, HttpClient, RequestParams } from "./http-client";

export class Api<SecurityDataType = unknown> extends HttpClient<SecurityDataType> {
  /**
   * No description
   *
   * @tags Auth
   * @name V1AuthClaimsList
   * @summary Gets the user's claims
   * @request GET:/api/v1/auth/claims
   * @secure
   */
  v1AuthClaimsList = (params: RequestParams = {}) =>
    this.request<Record<string, string>, any>({
      path: `/api/v1/auth/claims`,
      method: "GET",
      secure: true,
      format: "json",
      ...params,
    });
  /**
   * No description
   *
   * @tags Auth
   * @name V1AuthMeList
   * @summary Gets the current user
   * @request GET:/api/v1/auth/me
   * @secure
   */
  v1AuthMeList = (params: RequestParams = {}) =>
    this.request<User, any>({
      path: `/api/v1/auth/me`,
      method: "GET",
      secure: true,
      format: "json",
      ...params,
    });
  /**
   * No description
   *
   * @tags Auth
   * @name V1AuthLoginCreate
   * @summary Logs the user in
   * @request POST:/api/v1/auth/login
   * @secure
   */
  v1AuthLoginCreate = (data: LoginCommand, params: RequestParams = {}) =>
    this.request<User, any>({
      path: `/api/v1/auth/login`,
      method: "POST",
      body: data,
      secure: true,
      type: ContentType.Json,
      format: "json",
      ...params,
    });
  /**
   * @description Sample request: POST /register { "email": "elon@gmail.com", "fullname": "elonmusk", "password": "supersecure }
   *
   * @tags Auth
   * @name V1AuthRegisterCreate
   * @summary Register a new user
   * @request POST:/api/v1/auth/register
   * @secure
   */
  v1AuthRegisterCreate = (data: RegisterCommand, params: RequestParams = {}) =>
    this.request<User, any>({
      path: `/api/v1/auth/register`,
      method: "POST",
      body: data,
      secure: true,
      type: ContentType.Json,
      format: "json",
      ...params,
    });
  /**
 * No description
 *
 * @tags Auth
 * @name V1AuthAllUsersList
 * @summary Gets all users
<note>This is only for Elon</note>
 * @request GET:/api/v1/auth/all_users
 * @secure
 */
  v1AuthAllUsersList = (params: RequestParams = {}) =>
    this.request<User[], any>({
      path: `/api/v1/auth/all_users`,
      method: "GET",
      secure: true,
      format: "json",
      ...params,
    });
}
